using Akka;
using Akka.Actor;

namespace Actors
{
    public class WatchActor : ReceiveActor
    {
        public IActorRef FirstChild = Context.ActorOf(ChildActor.Props(), "first");

        public WatchActor()
        {
            Context.Watch(FirstChild);

            //var second = Context.ActorOf(ChildActor.Props(), "second");
            //Context.Watch(second);

            Receive<string>(s => s.Equals("kill"), msg =>
            {
                foreach (var child in Context.GetChildren())
                {
                    //Context.Stop(child);

                    child.Tell("kill");
                }
            });

            Receive<string>(s => s.Equals("realay"), msg =>
            {
                FirstChild.Tell(msg);
            });

            Receive<Terminated>(t => t.ActorRef.Equals(FirstChild), msg =>
            {
                System.Console.WriteLine($"Terminated : {Sender} (only for first child)");
            });

            Receive<Terminated>(msg =>
            {
                System.Console.WriteLine($"Terminated : {Sender}");
            });
        }

        public static Props Props()
        {
            return Akka.Actor.Props.Create<WatchActor>();
        }
    }
}