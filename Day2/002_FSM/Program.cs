using Akka.Actor;
using Akka.Configuration;

using Actors;
using FSM002.Structures.Commands;
using FSM002.Structures.Datas;

/*
스토리지 플러그인
커스텀 저널
사전 로드
*/

namespace FSM002
{
    class Program
    {
        static void Main(string[] args)
        {
            var system = ActorSystem.Create("MyActorSystem001");

            var repo_actor = system.ActorOf(ReportActor.Props(), "Report002");
            var shop_actor = system.ActorOf(FSMShopActor.Props(repo_actor), "Shop002");
            
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