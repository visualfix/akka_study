using Akka.Actor;
using Actors;
using Akka.Routing;
using Akka.Configuration;

namespace Msg002
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = ConfigurationFactory.Load();
            var system = ActorSystem.Create("MyMsgSystem002", config);

            var route_wokers = system.ActorOf(
                Worker.Props().WithRouter(FromConfig.Instance)
                //, "worker_pool");
                , "worker_resize_pool");

            //route_wokers.Tell(PoisonPill.Instance);    
            route_wokers.Tell(new Broadcast(PoisonPill.Instance));
            route_wokers.Tell("start");
            Thread.Sleep(1000);

            for(int i = 11; i > 0; i--)
            {
                route_wokers.Tell($"test{i}");
                Thread.Sleep(100);
            }

            Thread.Sleep(1000);

            route_wokers.Tell("end");
            
            Thread.Sleep(10000);
        }
    }
}