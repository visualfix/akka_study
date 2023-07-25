using Akka.Actor;
using Actors;

namespace Actor001
{
    class Program
    {
        static void Main(string[] args)
        {
            var system = ActorSystem.Create("MyActorSystem006");

            var manager = system.ActorOf(StopManager.Props(), "StopManager");

            try
            {
                await manager.GracefulStop(TimeSpan.FromMilliseconds(5), "shutdown");
                // the actor has been stopped
            }
            catch (TaskCanceledException)
            {
                // the actor wasn't stopped within 5 seconds
            }

            Thread.Sleep(10000);
        }
    }
}