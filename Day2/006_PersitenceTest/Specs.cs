using Xunit;
using Akka.Persistence.TestKit;
using Akka.Persistence;
using Akka.TestKit;
using Shouldly;
using PersitenceTest.Actors;
using Akka.Dispatch.SysMsg;
using Akka.Actor;

namespace PersitenceTest;

class MyInterceptor : IJournalInterceptor
{    
    private readonly Type _messageType;

    public MyInterceptor(Type messageType)
    {
        _messageType = messageType;
    }

    public Task InterceptAsync(IPersistentRepresentation message)
    {
        var type = message.Payload.GetType();

        if (_messageType.IsAssignableFrom(type))
        {
           throw new TestJournalFailureException(); 
        }

        return Task.FromResult(true);
    }
}

public class CounterActorTests : PersistenceTestKit
{
    [Fact]
    public async Task test1()
    {
        await WithJournalWrite(write => write.Pass(), () =>
        {
            var actor = ActorOf(() => new CounterActor("test"), "counter1");
            actor.Tell("inc", TestActor);
            actor.Tell("read", TestActor);

            var result = ExpectMsg<int>(TimeSpan.FromSeconds(3));
            result.ShouldBe(1);
        });
    }

    [Fact]
    public async Task test2()
    {
        await WithJournalWrite(write => write.SetInterceptorAsync(new MyInterceptor(typeof(int))), () =>
        {
            var actor = ActorOf(() => new CounterActor("test"), "counter2");
            actor.Tell("inc", TestActor);
            //actor.Tell(1, TestActor);
            actor.Tell("read", TestActor);

            var result = ExpectMsg<int>(TimeSpan.FromSeconds(3));
            result.ShouldBe(1);
        });
    }

    [Fact]
    public async Task test3()
    {
        await WithJournalWrite(write => write.Pass(), () =>
        {
            var actor = ActorOf(() => new CounterActor("test"), "counter3");
            actor.Tell("inc", TestActor);
            actor.Tell("read", TestActor);

            var result = ExpectMsg<int>(TimeSpan.FromSeconds(3));
            result.ShouldBe(1);
        });

        await WithJournalRecovery(write => write.Pass(), () =>
        {
            var actor = ActorOf(() => new CounterActor("test"), "counter4");
            actor.Tell("inc", TestActor);
            actor.Tell("read", TestActor);

            var result = ExpectMsg<int>(TimeSpan.FromSeconds(3));
            result.ShouldBe(2);
        });
    }
}
