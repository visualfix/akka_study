using Akka;
using Akka.Actor;

namespace Actors
{
    public class WatchActor : ReceiveActor
    {
        public WatchActor()
        {
            Context.ActorOf(ChildActor.Props(), "first");
            Context.ActorOf(ChildActor.Props(), "second");

            Receive<string>(s => s.Equals("WatchAll"), msg =>
            {
                foreach (var child in Context.GetChildren())
                {
                    Context.Watch(child);
                }
            });

            Receive<string>(s => s.Equals("KillAll"), msg =>
            {
                foreach (var child in Context.GetChildren())
                {
                    //Context.Stop(child);
                    child.Tell("kill");
                }
            });

            Receive<Terminated>(msg =>
            {
                System.Console.WriteLine($"Terminated : {msg.ActorRef}");
            });
        }

        public static Props Props()
        {
            return Akka.Actor.Props.Create<WatchActor>();
        }
    }
}