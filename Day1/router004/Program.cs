using Akka.Actor;
using Actors;
using Akka.Routing;
using Akka.Configuration;

namespace Router004
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = ConfigurationFactory.Load();
            var system = ActorSystem.Create("MyRouterSystem004", config);

            var route_wokers = system.ActorOf(
                Worker.Props().WithRouter(FromConfig.Instance)
                , "worker_hash_pool");
                
            int call_cnt = 10;
            while( call_cnt --> 0)
            {
                route_wokers.Tell(new MyMessage(call_cnt, $"message {call_cnt}"));
                Thread.Sleep(100);
            }
            
            Thread.Sleep(1000);
        }
    }
}