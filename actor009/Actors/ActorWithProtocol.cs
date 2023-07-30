using System;
using Akka;
using Akka.Actor;
using Akka.Event;

namespace Actors
{
    public class ActorWithProtocol : ReceiveActor, IWithStash
    {
        private readonly ILoggingAdapter _log = Logging.GetLogger(Context);
        public IStash Stash { get; set; }

        public ActorWithProtocol()
        {
            Receive<string>(s => s.Equals("release"), _ =>
            {
                Self.Tell("keep");
                Stash.UnstashAll();
                Become(Release);
            });

            ReceiveAny(_ => 
            {
                _log.Debug($"Stash : {_}");
                Stash.Stash();
            });
        }

        public void Release()
        {
            Receive<Write>(write =>
            {
                _log.Debug(write.Text);
            });

            Receive<string>(s => s.Equals("keep"), _ =>
            {
                UnbecomeStacked();
            });

            ReceiveAny(_ => 
            {
                _log.Debug($"Stash : {_}");
                Stash.Stash();
            });
        }


        public static Props Props()
        {
            return Akka.Actor.Props.Create<ActorWithProtocol>();
        }
    }
}