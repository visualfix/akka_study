using Akka;
using Akka.Actor;
using Akka.Event;
using Akka.Routing;

namespace Actors
{
  public class Worker: ReceiveActor
  {
    private readonly ILoggingAdapter _log = Logging.GetLogger(Context);
    public Worker()
    {
      Receive<string>(msg =>
      {
        _log.Debug($"{msg}");
      });
      
    }

    public static Props Props()
    {
      return Akka.Actor.Props.Create<Worker>();
    }
  }
}