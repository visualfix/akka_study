using Akka.Actor;
using Actors;

namespace Actor001
{
    class Program
    {
        static void Main(string[] args)
        {
            var system = ActorSystem.Create("MyActorSystem001");

            var receive_actor = system.ActorOf<RActor>("RActor001");

            //CASE1 
            //Props send_prop = Props.Create(typeof(SActor));

            //CASE2
            Props send_prop = Props.Create(()=> new SActor(receive_actor, "How are you?"));

            //CASE3
            //Props send_prop = Props.Create<SActor>(receive_actor, "How are you?");

            //CASE4
            //Props send_prop = Props.Create(typeof(SActor), receive_actor, "How are you?");

            var send_actor =system.ActorOf(send_prop, "SActor001");

            //CASE1
            // receive_actor.Tell("How are you?", send_actor);

            Thread.Sleep(1000);
            system.Stop(receive_actor);
            system.Stop(send_actor);       
            //Console.ReadLine();
        }
    }
}