using System;
using Akka;
using Akka.Actor;
using Akka.Event;

namespace Actors
{
    public class PingPong : ReceiveActor, ILogReceive
    {
        IActorRef? receiver_actor = null;
        private readonly ILoggingAdapter _log = Logging.GetLogger(Context);

        public PingPong()
        {
            _log.Debug($"Constructor");
            Receive<PingPongMessage>(msg =>
            {
                Thread.Sleep(300);

                receiver_actor?.Tell(new PingPongMessage());
            });

            Receive<SetPlayer>(msg =>
            {
                receiver_actor = msg.Player;
            });

            Receive<string>(s => s.Equals("start"), msg =>
            {
                _log.Debug($"Start Game");
                receiver_actor?.Tell(new PingPongMessage());
            });
            
            Receive<Shutdown>(_ =>
            {
                if(Context.GetChildren().FirstOrDefault() is null)
                {
                    _log.Debug("Stop");
                    Self.Tell(PoisonPill.Instance);
                }
                else
                {
                    foreach(var c in Context.GetChildren())
                    {
                        c.GracefulStop(TimeSpan.FromSeconds(5), new Shutdown());
                    }

                    Become(ShuttingDown);
                }
            });
        }

        private void ShuttingDown()
        {
            Receive<Terminated>(msg =>
            {
                Context.Unwatch(msg.ActorRef);
                if(Context.GetChildren().FirstOrDefault() is null)
                {
                    _log.Debug("Stop");
                    Self.Tell(PoisonPill.Instance);
                }
            });
        }

        public static Props Props()
        {
            return Akka.Actor.Props.Create<PingPong>();
        }


        protected override void PreStart()
        {
            _log.Debug("PreStart");
        }

        protected override void PreRestart(Exception reason, object message)
        {
            _log.Debug($"PreRestart \n reason : {reason} \n message : {message}");

            foreach (IActorRef each in Context.GetChildren())
            {
                Context.Unwatch(each);
                Context.Stop(each);
            }
            PostStop();
        }

        protected override void PostRestart(Exception reason)
        {
            _log.Debug($"PostRestart {reason}");

            PreStart();
        }

        protected override void PostStop()
        {
            _log.Debug("PostStop");
        }
    }
}