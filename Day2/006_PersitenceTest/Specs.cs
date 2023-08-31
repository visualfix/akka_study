using Xunit;
using Akka.Persistence.TestKit;
using Akka.Persistence;
using Akka.TestKit;
using Shouldly;
using PersitenceTest.Actors;

namespace PersitenceTest;

public class CounterActorTests : PersistenceTestKit
{
     private readonly TestProbe _probe;

    public CounterActorTests()
    {
        _probe = CreateTestProbe();
    }

    [Fact]
    public async Task test1()
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

    [Fact]
    public async Task journal_must_reset_state_to_pass()
    {
        await WithJournalWrite(write => write.Fail(), async () =>
        {
            var actor = ActorOf(() => new PersistActor(_probe));
            Watch(actor);
            await _probe.ExpectMsgAsync<RecoveryCompleted>();

            actor.Tell(new PersistActor.WriteMessage("write"), TestActor);
            await _probe.ExpectMsgAsync("failure");
            await ExpectTerminatedAsync(actor);
        });

        var actor2 = ActorOf(() => new PersistActor(_probe));
        Watch(actor2);

        await _probe.ExpectMsgAsync<RecoveryCompleted>();
        actor2.Tell(new PersistActor.WriteMessage("write"), TestActor);
        await _probe.ExpectMsgAsync("ack");
    }
}
