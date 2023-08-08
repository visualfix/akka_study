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

    Dictionary<int, int> ThreadCount = new Dictionary<int, int>();
    public Parent()
    {
      route_wokers = Context.ActorOf(
          Worker.Props().WithRouter(FromConfig.Instance), "worker_pool");

      route_wokers2 = Context.ActorOf(
          Worker.Props().WithRouter(new RoundRobinPool(5)).WithDispatcher("my-dispatcher2"), "worker_pool2");
          
      Receive<string>(s=>s.Equals("do!"), msg =>
      {
        int loop = 10;
        while(loop --> 0)
        {
          route_wokers.Tell("");
        }
        Thread.Sleep(1000);

        loop = 10;
        while(loop --> 0)
        {
          route_wokers2.Tell("");
        }
      });

      Receive<string>(s=>s.Equals("result!"), msg =>
      {
        foreach(var p in ThreadCount)
        {
          System.Console.WriteLine($"{p.Key} {p.Value}");
        }
      });
      
      Receive<Response>(msg =>
      {
        if(ThreadCount.ContainsKey(msg.TID) == false)
        {
          ThreadCount[msg.TID] = 0;
        }
        else
        {
          ThreadCount[msg.TID]++;
        }
      });

    }

    public static Props Props()
    {
      return Akka.Actor.Props.Create<Parent>();
    }
  }
}