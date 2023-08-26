using Akka.Actor;
using Akka.Configuration;

using FSM004.MyJournals;
using Akka.Persistence.Redis.Query;
using Akka.Persistence.Query;
using Akka.Streams.Dsl;
using Akka;
using Akka.Streams;

namespace FSM004
{
    class Program
    {
        static void Main(string[] args)
        {

            var system = ActorSystem.Create("MyActorSystem004");//, msgconfig);
            var readJournal = PersistenceQuery.Get(system).ReadJournalFor<RedisReadJournal>(RedisReadJournal.Identifier);

            // issue query to journal
            Source<EventEnvelope, NotUsed> source = readJournal
                .EventsByPersistenceId("test-jnjournal:persisted:my-stable-persistence-id", 0);

            // materialize stream, consuming events
            var mat = ActorMaterializer.Create(system);
            source.RunForeach(envelope =>
            {
                Console.WriteLine($"event {envelope}");
            }, mat);


            Thread.Sleep(10000);
        }
    }
}