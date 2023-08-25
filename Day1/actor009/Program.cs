using Akka.Actor;
using Actors;
using Akka.Configuration;

namespace Actor009
{
    class Program
    {
        /*
            # Stash         : 
            # Stash         : Capacity
        */

        static void Main(string[] args)
        {
            var logconfig = ConfigurationFactory.ParseString(@"
                akka {
                    loglevel = DEBUG
                }");
            var stashconfig = ConfigurationFactory.ParseString(@"
                akka {
                    actor.deployment{
                        ""/*"" {
                            stash-capacity = 2
                        }
                    }
                }").WithFallback(logconfig);

            var system = ActorSystem.Create("MyActorSystem009", logconfig);

            var statsh_actor = system.ActorOf(ActorWithProtocol.Props(), "ActorWithProtocol");
            //var statsh_actor = system.ActorOf(ActorWithProtocol.PropsWithCapacity(2), "ActorWithProtocol");

            statsh_actor.Tell("DoStash");
            statsh_actor.Tell(new Write("a"));
            statsh_actor.Tell(new Write("b"));
            statsh_actor.Tell(new Write("c"));
            statsh_actor.Tell("StopStash");

            statsh_actor.Tell("DoStash");
            statsh_actor.Tell(new Write("aa"));
            statsh_actor.Tell(new Write("bb"));
            statsh_actor.Tell(new Write("cc"));
            statsh_actor.Tell("StopStash");

            Thread.Sleep(1000);
        }
    }
}