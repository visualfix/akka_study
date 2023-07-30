using Akka.Actor;
using Actors;

namespace Actor001
{
    class Program
    {
        /*
            # Constructor   : Overloading
            # Props         : Constructor, ActorArgument
        */

        static void Main(string[] args)
        {
            var system = ActorSystem.Create("MyActorSystem001");

            var receive_actor = system.ActorOf<RActor>("RActor001");

            //Props send_prop = Props.Create(typeof(SActor));
            //var send_actor =system.ActorOf(send_prop, "SActor001");
            //receive_actor.Tell("How are you?", send_actor);

            Props send_prop = Props.Create(()=> new SActor(receive_actor, "How are you?"));
            //Props send_prop = Props.Create<SActor>(receive_actor, "How are you?");
            //Props send_prop = Props.Create(typeof(SActor), receive_actor, "How are you?");

            var send_actor =system.ActorOf(send_prop, "SActor001");

            Thread.Sleep(1000);
        }
    }
}