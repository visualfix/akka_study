using Akka.Actor;
using Actors;

namespace Actor011
{
    class Program
    {
        static void Main(string[] args)
        {
            var system = ActorSystem.Create("MyActorSystem011");

            var test_actor = system.ActorOf(TestActor.Props(), "TestActor");

            test_actor.Tell("test");
            test_actor.Tell("unknown");

            Thread.Sleep(1000);
        }
    }
}