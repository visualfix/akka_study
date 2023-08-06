using Akka.Actor;
using Actors;
using Akka.Routing;
using Akka.Configuration;

namespace Router001
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = ConfigurationFactory.Load();
            var system = ActorSystem.Create("MyRouterSystem001", config);

            //var route_wokers = system.ActorOf(
            //    Worker.Props().WithRouter(FromConfig.Instance)
            //    , "worker_pool");

            var route_wokers = system.ActorOf(
                Worker.Props().WithRouter(new RoundRobinPool(5))
                , "worker_pool");
                
            int call_cnt = 10;
            while( call_cnt --> 0)
            {
                route_wokers.Tell("test");
                Thread.Sleep(100);
            }
            
            Thread.Sleep(1000);
        }
    }
}