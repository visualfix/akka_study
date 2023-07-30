using Akka.Actor;
using Actors;

namespace Actor006
{
    class Program
    {
        /*
            # SetReceiveTimeout : ReceiveTimeout Message, Interval
        */

        static void Main(string[] args)
        {
            var system = ActorSystem.Create("MyActorSystem006");

            var actor = system.ActorOf(TimeoutActor.Props(), "TimeoutActor");
            actor.Tell("Hello");

            Thread.Sleep(5000);
        }
    }
}