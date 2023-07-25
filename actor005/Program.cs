using Akka.Actor;
using Actors;

namespace Actor001
{
    class Program
    {
        static void Main(string[] args)
        {
            var system = ActorSystem.Create("MyActorSystem005");

            var ask_actor = system.ActorOf(AskTestActor.Props(), "AskActor");
            //ask_actor.Tell("Execute");

            var tell_actor = system.ActorOf(TellTestActor.Props(), "TellActor");
            tell_actor.Tell("Execute");

            Thread.Sleep(1000);
        }
    }
}