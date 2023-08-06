using Akka;
using Akka.Actor;
using Akka.Event;

namespace Actors
{
  public class Worker: ReceiveActor
  {
    private readonly ILoggingAdapter _log = Logging.GetLogger(Context);
    public Worker()
    {
      Receive<string>(_ =>
      {
        _log.Debug($"{Self.Path.Name} from {Sender.Path.Name}");
      });
    }

    public static Props Props()
    {
      return Akka.Actor.Props.Create<Worker>();
    }
  }
}