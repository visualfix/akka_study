using Akka.Actor;
using Akka.Configuration;
using Actors;

namespace Actor004
{
    class Program
    {
        /*
            # ActorSelection    : 
            # ActorSelection    : Identify, id
            # Watch             : Terminated
        */

        static void Main(string[] args)
        {
            var lifecycleconfig = ConfigurationFactory.ParseString(@"
                akka {
                    actor.debug
                    {
                        lifecycle = on
                    }
                }");

            var system = ActorSystem.Create("MyActorSystem004");//, lifecycleconfig);

            var parent_king = system.ActorOf(KingParent.Props(), "King");
            var parent_queen = system.ActorOf(QueenParent.Props(), "Queen");
            var parent_jack = system.ActorOf(JackParent.Props(), "Jack");

            //var selection = system.ActorSelection("/user/*");
            //var selection = system.ActorSelection("/user/King");
            //var selection = system.ActorSelection("/user/King/*");
            //selection.Tell("selected!");

            var killer = system.ActorOf(ActorFinder.Props(), "Killer");
            //killer.Tell("/user/King");
            killer.Tell("/user/King/*");

            Thread.Sleep(1000);
        }
    }
}