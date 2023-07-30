using Akka.Actor;
using Actors;

namespace Actor002
{
    class Program
    {
        /*
            # BestPractice  : 
            # InnerMessage  : 
        */

        static void Main(string[] args)
        {
            var system = ActorSystem.Create("MyActorSystem002");

            var receive_actor = system.ActorOf(RActor.Props(), "RActor002");

            //CASE 1
            var send_actor =system.ActorOf(SActor.Props(receive_actor, "How are you?"), "SActor002");

            //CASE 2
            //var send_actor =system.ActorOf(SActor.Props(), "SActor002");
            //var public_msg = new PublicMessage("Where am I?");
            //receive_actor.Tell(public_msg, send_actor);

            Thread.Sleep(1000);
        }
    }
}