using Actors;
using Akka.Actor;
using Akka.Configuration;

namespace Actor010
{
    class Program
    {
        /*
            # kill          : 
            # Supervisor    : All4One, One4One
            # Supervisor    : Stop, Restart
            # lifecycle     : PostRestart
        */

        static void Main(string[] args)
        {
            var logconfig = ConfigurationFactory.ParseString(@"
                akka {
                    loglevel = DEBUG
                }");
            var lifecycleconfig = ConfigurationFactory.ParseString(@"
                akka {
                    actor.debug
                    {
                        lifecycle = on
                    }
                }").WithFallback(logconfig);

            var system = ActorSystem.Create("MyActorSystem010", lifecycleconfig);

            var actor = system.ActorOf(SupervisorActor.PropsWithOne4One(), "SupervisorActor");
            actor.Tell("do kill");

            Thread.Sleep(1000);
        }
    }
}