using Akka.Actor;
using Actors;
using Akka.Routing;
using Akka.Configuration;

namespace Msg004
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = ConfigurationFactory.Load();
            var system = ActorSystem.Create("MyMsgSystem004", config);

            var parent = system.ActorOf(Parent.Props(), "parent");
            //parent.Tell("adjust!");
            parent.Tell("add!");
            //parent.Tell("remove!");
            
            Thread.Sleep(1000); 
        }
    }
}