using System;
using Akka.Persistence;
using Akka.Actor;



public class PersistActor : UntypedPersistentActor
{
    public PersistActor(IActorRef probe)
    {
        _probe = probe;
    }

    private readonly IActorRef _probe;

    public override string PersistenceId  => "foo";

    protected override void OnCommand(object message)
    {
        switch (message)
        {
            case WriteMessage msg:
                Persist(msg.Data, _ =>
                {
                    _probe.Tell("ack");
                });
                
                break;
            
            default:
                return;
        }
    }

    protected override void OnRecover(object message)
    {
        _probe.Tell(message);
    }

    protected override void OnPersistFailure(Exception cause, object @event, long sequenceNr)
    {
        _probe.Tell("failure");

        base.OnPersistFailure(cause, @event, sequenceNr);
    }

    protected override void OnPersistRejected(Exception cause, object @event, long sequenceNr)
    {
        _probe.Tell("rejected");

        base.OnPersistRejected(cause, @event, sequenceNr);
    }

    public class WriteMessage
    {
        public string Data { get; }

        public WriteMessage(string data)
        {
            Data = data;
        }
    }
}