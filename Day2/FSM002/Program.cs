using Akka.Actor;
using Akka.Configuration;

using Actors;
using FSM001.Structures.Commands;
using FSM001.Structures.Datas;


namespace FSM001
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = ConfigurationFactory.ParseString(@"
            akka {
                persistence.journal {
                    plugin = ""akka.persistence.journal.inmem2""
                    inmem2 {
                        class = ""MyCustomJournal.MyJournal, FSM001""
                        plugin-dispatcher = ""akka.actor.default-dispatcher""
                    }
                    auto-start-journals = [""akka.persistence.journal.inmem2""]
                }
            }");

            var logconfig = ConfigurationFactory.ParseString(@"
                akka {
                    loglevel = DEBUG
                }").WithFallback(config);
            var msgconfig = ConfigurationFactory.ParseString(@"
                akka {
                    actor.debug
                    {
                        receive = on
                        autoreceive = on
                    }
                }").WithFallback(logconfig);

            var system = ActorSystem.Create("MyActorSystem001", msgconfig);

            var repo_actor = system.ActorOf(ReportActor.Props(), "Report001");
            var shop_actor = system.ActorOf(FSMShopActor.Props(repo_actor), "Shop001");
            
            shop_actor.Tell(new AddItem(new Item("item01", "아이템1", 100)));
            shop_actor.Tell(new AddItem(new Item("item02", "아이템2", 100)));
            shop_actor.Tell(new AddItem(new Item("item03", "아이템3", 100)));
            shop_actor.Tell(Buy.Instance);
            shop_actor.Tell(Leave.Instance);

            Thread.Sleep(10000);

            //shop_actor = system.ActorOf(FSMShopActor.Props(repo_actor), "Shop002");
            //
            //shop_actor.Tell(new AddItem(new Item("item04", "아이템4", 100)));
            //shop_actor.Tell(new AddItem(new Item("item05", "아이템5", 100)));
            //shop_actor.Tell(new AddItem(new Item("item06", "아이템6", 100)));
            //Thread.Sleep(1000);
        }
    }
}