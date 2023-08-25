using Akka.Actor;
using Actors;
using Akka.Routing;
using Akka.Configuration;

namespace Router007
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = ConfigurationFactory.Load();
            var system = ActorSystem.Create("MyRouterSystem007", config);

            var parent = system.ActorOf(Parent.Props(), "parent");
            parent.Tell("do!");
            parent.Tell("do!");
            parent.Tell("do!");
            parent.Tell("do!");
            parent.Tell("do!");
            parent.Tell("do!");
            
            Thread.Sleep(10000); 
        }
    }
}