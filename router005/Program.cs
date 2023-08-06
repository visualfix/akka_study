using Akka.Actor;
using Actors;
using Akka.Routing;
using Akka.Configuration;

namespace Router003
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = ConfigurationFactory.Load();
            var system = ActorSystem.Create("MyRouterSystem003", config);

            var parent = system.ActorOf(Parent.Props(), "parent");
            parent.Tell("do!");
            
            Thread.Sleep(10000);
        }
    }
}