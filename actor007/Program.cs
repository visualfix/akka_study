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
            var msgconfig = ConfigurationFactory.ParseString(@"
                akka {
                    actor.debug
                    {
                        receive = on
                        autoreceive = on
                        lifecycle = on
                    }
                }").WithFallback(logconfig);

            var system = ActorSystem.Create("MyActorSystem007", msgconfig);

            var manager = system.ActorOf(PingPongManager.Props(), "PingPongManager");
            Thread.Sleep(2000);

            graceful_stop(manager);
        
            Thread.Sleep(5000);
        }

        static async void graceful_stop(IActorRef manager)
        {
            try
            {
                System.Console.WriteLine("Stop All");
                var result = await manager.GracefulStop(TimeSpan.FromSeconds(1), new Shutdown());
                System.Console.WriteLine("done!" + result);
                
            }
            catch (TaskCanceledException)
            {
                // the actor wasn't stopped within 5 seconds
            }
        } 
    }
}