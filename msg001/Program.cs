using Akka.Actor;
using Actors;
using Akka.Routing;
using Akka.Configuration;

namespace Msg001
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = ConfigurationFactory.Load();
            var system = ActorSystem.Create("MyMsgSystem001", config);

            var route_wokers = system.ActorOf(
                Worker.Props().WithRouter(FromConfig.Instance)
                , "worker_pool");
                
            route_wokers.Tell(new Broadcast("Hello world!"));
            
            Thread.Sleep(1000);
        }
    }
}