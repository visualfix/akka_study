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

            var system = ActorSystem.Create("MyActorSystem009", logconfig);

            var statsh_actor = system.ActorOf(ActorWithProtocol.Props(), "ActorWithProtocol");

            //statsh_actor.Tell("keep");
            statsh_actor.Tell(new Write("a"));
            statsh_actor.Tell(new Write("b"));
            statsh_actor.Tell(new Write("c"));
            statsh_actor.Tell("release");

            //statsh_actor.Tell("keep");
            statsh_actor.Tell(new Write("aa"));
            statsh_actor.Tell(new Write("bb"));
            statsh_actor.Tell(new Write("cc"));
            //statsh_actor.Tell("release");

            Thread.Sleep(1000);
        }
    }
}