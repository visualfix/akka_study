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
      Receive<MyMessage>(msg =>
      {
        _log.Debug($"{Self.Path.Name} : {msg.Text}");
      });

    }

    public static Props Props()
    {
      return Akka.Actor.Props.Create<Worker>();
    }
  }
}