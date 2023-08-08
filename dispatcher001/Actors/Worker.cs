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
      Receive<string>(msg =>
      {
        //_log.Debug($"{Thread.CurrentThread.ManagedThreadId}");
        //Thread.Sleep(200);
        Sender.Tell(new Response(Thread.CurrentThread.ManagedThreadId));
      });

    }

    public static Props Props()
    {
      return Akka.Actor.Props.Create<Worker>();
    }
  }
}