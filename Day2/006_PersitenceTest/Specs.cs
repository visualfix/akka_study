using Akka;
using Akka.TestKit.Xunit2;
using Xunit;
using Akka.Persistence.TestKit;
using Akka.TestKit;
using Shouldly;

namespace FSM004
{
    public class CounterActorTests : PersistenceTestKit
    {
        [Fact]
        public async Task CounterActor_internal_state_will_be_lost_if_underlying_persistence_store_is_not_available()
        {
            await WithJournalWrite(write => write.Fail(), () =>
            {
                var actor = ActorOf(() => new CounterActor("test"), "counter");
                actor.Tell("inc", TestActor);
                actor.Tell("read", TestActor);

                var result = ExpectMsg<int>(TimeSpan.FromSeconds(3));
                result.ShouldBe(0);
            });
        }
    }
}