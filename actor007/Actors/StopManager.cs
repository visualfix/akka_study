using System;
using Akka;
using Akka.Actor;

namespace Actors
{

    public class StopManager : ReceiveActor
    {
        private IActorRef worker = Context.Watch(Context.ActorOf<Cruncher>("worker"));

        public StopManager()
        {
            Receive<string>(s => s.Equals("job"), msg =>
            {
                worker.Tell("crunch");
            });

            Receive<Shutdown>(_ =>
            {
                worker.Tell(PoisonPill.Instance, Self);
                Context.Become(ShuttingDown);
            });
        }

        private void ShuttingDown(object message)
        {
            Receive<string>(s => s.Equals("job"), msg =>
            {
                Sender.Tell("service unavailable, shutting down", Self);
            });

            Receive<Shutdown>(_ =>
            {
                Context.Stop(Self);
            });
        }

        public static Props Props()
        {
            return Akka.Actor.Props.Create<StopManager>();
        }
    }
}