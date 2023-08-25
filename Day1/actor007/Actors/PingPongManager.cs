using System;
using Akka;
using Akka.Actor;
using Akka.Event;

namespace Actors
{
    public class PingPongManager : ReceiveActor
    {
        private readonly ILoggingAdapter _log = Logging.GetLogger(Context);

        public PingPongManager()
        {
            var player1 = Context.Watch(Context.ActorOf(PingPong.Props(), "player1"));
            var player2 = Context.Watch(Context.ActorOf(PingPong.Props(), "player2"));

            player1.Tell(new SetPlayer(player2));
            player2.Tell(new SetPlayer(player1));

            player1.Tell("start");

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
            return Akka.Actor.Props.Create<PingPongManager>();
        }
    }
}