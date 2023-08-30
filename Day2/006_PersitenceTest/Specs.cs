using Xunit;
using Akka.Persistence.TestKit;
using Shouldly;
using PersitenceTest.Actors;

namespace PersitenceTest;

public class CounterActorTests : PersistenceTestKit
{
    [Fact]
    public async Task CounterActor_internal_state_will_be_lost_if_underlying_persistence_store_is_not_available()
    {
        await WithJournalWrite(write => write.Pass(), () =>
        {
            var actor = ActorOf(() => new CounterActor("test"), "counter");
            actor.Tell("inc", TestActor);
            actor.Tell("read", TestActor);

            var result = ExpectMsg<int>(TimeSpan.FromSeconds(3));
            result.ShouldBe(1);
        });
    }
}
