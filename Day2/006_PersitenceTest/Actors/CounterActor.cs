using Akka.Actor;
using Akka.Persistence;

public class CounterActor : UntypedPersistentActor
{
    public CounterActor(string id)
    {
        PersistenceId = id;
    }

    private int value = 0;

    public override string PersistenceId { get; }

    protected override void OnCommand(object message)
    {
        switch (message as string)
        {
            case "inc":
                value++;
                Persist(message, _ => { });
                break;

            case "dec":
                value++;
                Persist(message, _ => { });
                break;

            case "read":
                Sender.Tell(value, Self);
                break;

            default:
                return;
        }
    }

    protected override void OnRecover(object message)
    {
        switch (message as string)
        {
            case "inc":
                value++;
                break;

            case "dec":
                value++;
                break;

            default:
                return;
        }
    }
}