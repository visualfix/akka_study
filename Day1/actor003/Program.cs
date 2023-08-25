using Akka.Actor;
using Actors;

namespace Actor003
{
    class Program
    {
        /*
            # lifecycle     : 
            # WatchAll      :
            # KillAll       : Stop(child), Stop(self)
            # Stop Watcher  :
        */

        static void Main(string[] args)
        {
            var system = ActorSystem.Create("MyActorSystem003");

            var watch_actor = system.ActorOf(WatchActor.Props(), "WatchActor003");
            watch_actor.Tell("WatchAll");

            watch_actor.Tell("KillAll");

            Thread.Sleep(1000);

            //system.Stop(watch_actor);

            Thread.Sleep(1000);
        }
    }
}