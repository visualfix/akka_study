using Akka;
using Akka.Actor;
using Akka.Event;
using Akka.Routing;

namespace Actors
{
  public class Parent: ReceiveActor
  {
    private readonly ILoggingAdapter _log = Logging.GetLogger(Context);
    Router route_wokers; 

    public Parent()
    {
      var routees = Enumerable.Range(1, 5)
          .Select(i => new ActorRefRoutee(Context.ActorOf<Worker>("w" + i))).ToArray();

      route_wokers = new Router(new RoundRobinRoutingLogic(), routees);

      Receive<string>(s=>s.Equals("do!"), msg =>
      {
        for (var i = 0; i < 10; i++)
        {
          route_wokers.Route("msg #" + i, ActorRefs.NoSender);
        }
      });
    }

    public static Props Props()
    {
      return Akka.Actor.Props.Create<Parent>();
    }
  }
}