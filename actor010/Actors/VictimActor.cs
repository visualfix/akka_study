
using Akka.Actor;
using Akka.Event;

namespace Actors
{
    public class VictimActor : ReceiveActor
    {
        private readonly ILoggingAdapter _log = Logging.GetLogger(Context);

        public VictimActor()
        {
        }

        public static Props Props()
        {
            return Akka.Actor.Props.Create<VictimActor>();
        }

        //protected override void PostRestart(Exception reason)
        //{
        //}

        protected override void PreStart()
        {
            _log.Debug("VictimActor Init");
        }
    }
}