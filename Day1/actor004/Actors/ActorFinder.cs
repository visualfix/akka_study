using Akka;
using Akka.Actor;

namespace Actors
{
    public class ActorFinder : ReceiveActor
    {
        private int identifyId = 123456789;
        private IActorRef victim;

        public ActorFinder()
        {
            Receive<string>(path =>
            {
                var selection = Context.ActorSelection(path);
                selection.Tell(new Identify(identifyId), Self);
            });

            Receive<ActorIdentity>(identity =>
            {
                if (identity.MessageId.Equals(identifyId))
                {
                    var subject = identity.Subject;

                    if (subject == null)
                    {
                        Context.Stop(Self);
                    }
                    else
                    {
                        System.Console.WriteLine($"Find - {subject}");
                        victim = subject;
                        Context.Watch(victim);
                        Context.Stop(victim);
                    }
                }
            });

            Receive<Terminated>(t =>
            {
                Context.Unwatch(t.ActorRef);
                System.Console.WriteLine($"Terminated - {t.ActorRef}");
            });
        }

        public static Props Props()
        {
            return Akka.Actor.Props.Create<ActorFinder>();
        }
    }
}