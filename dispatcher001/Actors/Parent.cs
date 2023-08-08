using Akka;
using Akka.Actor;
using Akka.Event;
using Akka.Routing;

namespace Actors
{
  public class Parent: ReceiveActor
  {
    private readonly ILoggingAdapter _log = Logging.GetLogger(Context);
    IActorRef route_wokers; 
    IActorRef route_wokers2;
    IActorRef route_wokers3;

    Dictionary<int, int> ThreadCount = new Dictionary<int, int>();
    Dictionary<int, int> ThreadCount2 = new Dictionary<int, int>();
    Dictionary<int, int> ThreadCount3 = new Dictionary<int, int>();
    public Parent()
    {
      route_wokers = Context.ActorOf(
          Worker.Props(1).WithRouter(FromConfig.Instance).WithDispatcher("my-dispatcher"), "worker_pool1");

      route_wokers2 = Context.ActorOf(
          Worker.Props(2).WithRouter(FromConfig.Instance).WithDispatcher("my-dispatcher"), "worker_pool2");

      route_wokers3 = Context.ActorOf(
          Worker.Props(3).WithRouter(FromConfig.Instance).WithDispatcher("my-dispatcher2"), "worker_pool3");

      Receive<string>(s=>s.Equals("do!"), msg =>
      {
        var tasks = new List<Task<Response>>();

        int loop = 3000;
        while(loop --> 0)
        {
            tasks.Add(route_wokers.Ask<Response>("", TimeSpan.FromSeconds(100)));
            tasks.Add(route_wokers2.Ask<Response>("", TimeSpan.FromSeconds(100)));
            tasks.Add(route_wokers3.Ask<Response>("", TimeSpan.FromSeconds(100)));
        }
        Task.WhenAll(tasks).PipeTo(Self, Self);//, ()=>{return "result!";}, (e)=>{return "fail";});
      });

      Receive<string>(s=>s.Equals("result!"), msg =>
      {
        System.Console.WriteLine($"TID  P1 P2");
        foreach(var p in ThreadCount)
        {
          System.Console.WriteLine($"{p.Key, 3} {p.Value, 3} {ThreadCount2[p.Key], 2}");
        }
      });
      
      Receive<Response[]>(msgs =>
      {
        System.Console.WriteLine($"Done");
        foreach(var msg in msgs)
        {
          if(ThreadCount.ContainsKey(msg.TID) == false)
          {
            ThreadCount[msg.TID] = 0;
            ThreadCount2[msg.TID] = 0;
            ThreadCount3[msg.TID] = 0;
          }

          switch(msg.Index)
          {
            case 1:
              ThreadCount[msg.TID]++;
            break;
            case 2:
              ThreadCount2[msg.TID]++;
            break;
            case 3:
              ThreadCount3[msg.TID]++;
            break;
          }
        }

        System.Console.WriteLine($"TID  P1 P2");
        foreach(var p in ThreadCount)
        {
          System.Console.WriteLine($"{p.Key, 3} {p.Value, 3} {ThreadCount2[p.Key], 2} {ThreadCount3[p.Key], 2}");
        }
      });

    }

    public static Props Props()
    {
      return Akka.Actor.Props.Create<Parent>();
    }
  }
}