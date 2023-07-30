using Akka.Actor;
using Actors;
using Akka.Configuration;

namespace Actor001
{
    class Program
    {
        static void Main(string[] args)
        {
            var logconfig = ConfigurationFactory.ParseString(@"
                akka {
                    loglevel = DEBUG
                }");

            var system = ActorSystem.Create("MyActorSystem007", logconfig);

            var animals = system.ActorOf(AnimalSounds.Props(), "AnimalSounds");
            
            animals.Tell(new Play());
            animals.Tell(new Swap());
            animals.Tell(new Play());
            animals.Tell(new Swap());
            animals.Tell(new Play());
            animals.Tell(new Pop());
            animals.Tell(new Play());

            animals.Tell(new Push());
            animals.Tell(new Play());
            animals.Tell(new Push());
            animals.Tell(new Play());
            animals.Tell(new Pop());
            animals.Tell(new Play());
            animals.Tell(new Pop());
            animals.Tell(new Play());

            Thread.Sleep(1000);
        }
    }
}