using System;
using Akka;
using Akka.Actor;
using Akka.Event;

namespace Actors
{
    public class ActorWithProtocol : ReceiveActor, IWithStash, ILogReceive 
    {
        private readonly ILoggingAdapter _log = Logging.GetLogger(Context);
        public IStash Stash { get; set; }

        public ActorWithProtocol()
        {
            Become(StopStash);
        }

        public void StopStash()
        {
            Receive<Write>(write =>
            {
                _log.Debug(write.Text);
            });

            Receive<string>(s => s.Equals("DoStash"), _ =>
            {
                Become(DoStash);
            });
        }

        public void DoStash()
        {
            Receive<string>(s => s.Equals("StopStash"), _ =>
            {
                Stash.UnstashAll();
                Become(StopStash);
            });

            ReceiveAny(msg => 
            {
                _log.Debug($"Stash {msg}");
                Stash.Stash();
            });
        }


        public static Props Props()
        {
            return Akka.Actor.Props.Create<ActorWithProtocol>();
        }

        public static Props PropsWithCapacity(int cap)
        {
            return Akka.Actor.Props.Create<ActorWithProtocol>().WithStashCapacity(cap);
        }
    }
}