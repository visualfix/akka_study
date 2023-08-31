using Akka.Actor;
using Akka.Persistence.Redis.Query;
using Akka.Persistence.Query;
using Akka.Streams.Dsl;
using Akka;
using Akka.Streams;
using FSM.Structures.Events.Domains;
using FSM.Structures.Datas;
using FSM.Actors;
using PersitenceQuery.MyJournals;

namespace PersitenceQuery
{
    class Program
    {
        static void Main(string[] args)
        {

            var temp = new ItemAdded(new Item("aa", "bbb", 100));

            var system = ActorSystem.Create("MyActorSystem004");//, msgconfig);

            //var repo_actor = system.ActorOf(ReportActor.Props(), "Report002");
            //var shop_actor = system.ActorOf(FSMShopActor.Props(repo_actor), "Shop002");

            var readJournal = PersistenceQuery.Get(system).ReadJournalFor<MyJournal>(MyJournal.Identifier);

            // issue query to journal
            Source<string, NotUsed> source = readJournal.PersistenceIds();

            // materialize stream, consuming events
            var mat = ActorMaterializer.Create(system);
            source.RunForeach(envelope =>
            {
                Console.WriteLine($"event {envelope}");
            }, mat);


            Thread.Sleep(60000);
        }
    }
}