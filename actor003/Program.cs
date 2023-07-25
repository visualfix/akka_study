using Akka.Actor;
using Actors;

namespace Actor001
{
    class Program
    {
        static void Main(string[] args)
        {
            var system = ActorSystem.Create("MyActorSystem003");

            var watch_actor = system.ActorOf(WatchActor.Props(), "WatchActor003");
            watch_actor.Tell("kill");

            Thread.Sleep(1000);

            watch_actor.Tell("realay");
            Thread.Sleep(1000);
            
            system.Stop(watch_actor);

        }
    }
}