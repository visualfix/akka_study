using Akka;
using Akka.Actor;
using Akka.Event;

namespace Actors
{
  public class ParentWorker: ReceiveActor
  {
    private readonly ILoggingAdapter _log = Logging.GetLogger(Context);
    public ParentWorker(int child_cnt)
    {
      while(child_cnt --> 0)
      {
        Context.ActorOf(Worker.Props(), $"w{child_cnt}");
        _log.Debug($"make : w{child_cnt}");
      }
    }

    public static Props Props(int child_cnt)
    {
      return Akka.Actor.Props.Create<ParentWorker>(child_cnt);
    }
  }
}