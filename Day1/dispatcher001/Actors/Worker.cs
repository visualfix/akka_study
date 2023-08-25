using Akka;
using Akka.Actor;
using Akka.Event;

namespace Actors
{
  public class Worker: ReceiveActor
  {
    int Index;
    private readonly ILoggingAdapter _log = Logging.GetLogger(Context);
    public Worker(int idx)
    {
      Index = idx;
      Receive<string>(msg =>
      {
        //_log.Debug(msg);
        Thread.Sleep(10);
        Sender.Tell(new Response(Index, Thread.CurrentThread.ManagedThreadId));
      });
    }

    public static Props Props(int idx)
    {
      return Akka.Actor.Props.Create<Worker>(idx);
    }
  }
}