using Akka.Actor;
using Actors;

namespace Actor001
{
    class Program
    {
        static void Main(string[] args)
        {
            var system = ActorSystem.Create("MyActorSystem006");

            var actor = system.ActorOf(TimeoutActor.Props(), "TimeoutActor");
            actor.Tell("Hello");

            Thread.Sleep(10000);
        }
    }
}