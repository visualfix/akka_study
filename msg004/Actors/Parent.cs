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

    IActorRef custom_woker; 
    public Parent()
    {
      route_wokers = Context.ActorOf(
          Worker.Props().WithRouter(FromConfig.Instance)
          , "worker_pool");

      custom_woker = Context.ActorOf(Worker.Props(), "custom_woker");
          
      Receive<string>(s=>s.Equals("adjust!"), msg =>
      {
        route_wokers.Tell(new AdjustPoolSize(2));
        route_wokers.Tell(new GetRoutees());
      });

      Receive<string>(s=>s.Equals("add!"), msg =>
      {
        route_wokers.Tell(new AddRoutee(Routee.FromActorRef(custom_woker)));
        route_wokers.Tell(new GetRoutees());
      });

      Receive<string>(s=>s.Equals("remove!"), msg =>
      {
        route_wokers.Tell(new RemoveRoutee(Routee.FromActorRef(custom_woker)));
        route_wokers.Tell(new GetRoutees());
      });
      
      Receive<Routees>(rts =>
      {
        // Routee -> ActorRefRoutee
        foreach(ActorRefRoutee routee in rts.Members)
        {
          _log.Debug($"GetRoutees : {routee.Actor.Path}");
        }
        
      });

    }

    public static Props Props()
    {
      return Akka.Actor.Props.Create<Parent>();
    }
  }
}