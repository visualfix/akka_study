using Akka.Actor;
using Akka.Configuration;

using Actors;
using FSM.Structures.Commands;
using FSM.Structures.Datas;

/*
    이벤트 + 데이터 2개씩 저장됨
    이때문에 항상 정배수에 스냅샷이 찍히는게 아님
*/
namespace FSM
{
    class Program
    {
        static void Main(string[] args)
        {
            var system = ActorSystem.Create("MyActorSystem003");

            var repo_actor = system.ActorOf(ReportActor.Props(), "Report003");
            var shop_actor = system.ActorOf(FSMShopActor.Props(repo_actor), "Shop003");
            
            shop_actor.Tell(new AddItem(new Item("item01", "아이템1", 100)));
            Console.ReadLine();

            shop_actor.Tell(new AddItem(new Item("item02", "아이템2", 100)));
            Console.ReadLine();

            shop_actor.Tell(new AddItem(new Item("item03", "아이템3", 100)));
            Console.ReadLine();

            Thread.Sleep(10000);
        }
    }
}