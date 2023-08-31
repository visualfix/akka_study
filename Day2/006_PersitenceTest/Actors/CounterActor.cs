using Akka.Persistence;

namespace PersitenceTest.Actors;

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
        if(message is string)
        {
            var msg = message as string;
            switch (msg)
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
            }
        }
        else
        {
            Persist(message, _ => { });
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