using Akka.Actor;
using Actors;
using Akka.Routing;
using Akka.Configuration;

namespace Dispatcher001
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = ConfigurationFactory.Load();
            var system = ActorSystem.Create("MyDispatcherSystem007", config);

            var parent = system.ActorOf(Parent.Props(), "parent");
            parent.Tell("do!");
            Thread.Sleep(100000); 
            //parent.Tell("result!");            
            Thread.Sleep(1000); 
        }
    }
}