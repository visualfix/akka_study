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

            var system = ActorSystem.Create("MyActorSystem004");

            var readJournal = PersistenceQuery.Get(system).ReadJournalFor<MyJournal>(MyJournal.Identifier);

            Source<string, NotUsed> source = readJournal.PersistenceIds();
            var mat = ActorMaterializer.Create(system);
            
            source.RunForeach(envelope =>
            {
                Console.WriteLine($"event {envelope}");
            }, mat);


            Thread.Sleep(60000);
        }
    }
}