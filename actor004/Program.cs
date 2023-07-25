using Akka.Actor;
using Actors;

namespace Actor001
{
    class Program
    {
        static void Main(string[] args)
        {
            var system = ActorSystem.Create("MyActorSystem004");

            var parent_king = system.ActorOf(KingParent.Props(), "King");
            var parent_queen = system.ActorOf(QueenParent.Props(), "Queen");
            var parent_jack = system.ActorOf(JackParent.Props(), "Jack");

            //var selection = system.ActorSelection("/user/*");
            //selection.Tell("selected!");

            //var selection = system.ActorSelection("/user/*");
            //selection.Tell("selected!");

            //var selection = system.ActorSelection("/user/*");
            //selection.Tell("selected!");

            var killer = system.ActorOf(ActorFinder.Props(), "Killer");

            killer.Tell("/user/King");

            Thread.Sleep(1000);
            system.Stop(parent_king);
            system.Stop(parent_queen);
            system.Stop(parent_jack);

        }
    }
}