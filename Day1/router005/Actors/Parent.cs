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
    public Parent()
    {
      route_wokers = Context.ActorOf(
          Worker.Props().WithRouter(FromConfig.Instance)
          , "worker_tc_pool");
          
      Receive<string>(s=>s.Equals("do!"), msg =>
      {
        route_wokers.Tell("test");
      });
      

      Receive<Response>(msg =>
      {
        _log.Debug($"{msg} from {msg.Sender.Path.Name}");
      });

    }

    public static Props Props()
    {
      return Akka.Actor.Props.Create<Parent>();
    }
  }
}