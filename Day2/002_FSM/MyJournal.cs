
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using Akka.Util.Internal;
using Akka.Persistence;
using Akka.Persistence.Journal;

namespace FSM.MyCustomJournal;

using Messages = IDictionary<string, LinkedList<IPersistentRepresentation>>;

public class MyJournal : AsyncWriteJournal
{
    private readonly LinkedList<IPersistentRepresentation> _allMessages = new();
    private readonly ConcurrentDictionary<string, LinkedList<IPersistentRepresentation>> _messages = new();
    private readonly ConcurrentDictionary<string, long> _meta = new();
    private readonly ConcurrentDictionary<string, LinkedList<IPersistentRepresentation>> _tagsToMessagesMapping = new();
    
    /// <summary>
    /// TBD
    /// </summary>
    protected virtual ConcurrentDictionary<string, LinkedList<IPersistentRepresentation>> Messages { get { return _messages; } }


    /// <summary>
    /// TBD
    /// </summary>
    /// <param name="messages">TBD</param>
    /// <returns>TBD</returns>
    protected override Task<IImmutableList<Exception>> WriteMessagesAsync(IEnumerable<AtomicWrite> messages)
    {
        System.Console.WriteLine("Call - WriteMessagesAsync");
        foreach (var w in messages)
        {
            foreach (var p in (IEnumerable<IPersistentRepresentation>)w.Payload)
            {
                var persistentRepresentation = p.WithTimestamp(DateTime.UtcNow.Ticks);
                Add(persistentRepresentation);
                _allMessages.AddLast(persistentRepresentation);
                if (!(p.Payload is Tagged tagged)) continue;
                
                foreach (var tag in tagged.Tags)
                {
                    _tagsToMessagesMapping.AddOrUpdate(
                        tag,
                        (_) => new LinkedList<IPersistentRepresentation>(new[] { persistentRepresentation }),
                        (_, v) =>
                        {
                            v.AddLast(persistentRepresentation);
                            return v;
                        });
                }
            }
        }
        
        return Task.FromResult((IImmutableList<Exception>) null); // all good
    }

    /// <summary>
    /// TBD
    /// </summary>
    /// <param name="persistenceId">TBD</param>
    /// <param name="fromSequenceNr">TBD</param>
    /// <returns>TBD</returns>
    public override Task<long> ReadHighestSequenceNrAsync(string persistenceId, long fromSequenceNr)
    {
        System.Console.WriteLine("Call - ReadHighestSequenceNrAsync");
        return Task.FromResult(Math.Max(HighestSequenceNr(persistenceId), _meta.TryGetValue(persistenceId, out long metaSeqNr) ? metaSeqNr : 0L));
    }

    /// <summary>
    /// TBD
    /// </summary>
    /// <param name="context">TBD</param>
    /// <param name="persistenceId">TBD</param>
    /// <param name="fromSequenceNr">TBD</param>
    /// <param name="toSequenceNr">TBD</param>
    /// <param name="max">TBD</param>
    /// <param name="recoveryCallback">TBD</param>
    /// <returns>TBD</returns>
    public override Task ReplayMessagesAsync(IActorContext context, string persistenceId, long fromSequenceNr, long toSequenceNr, long max,
        Action<IPersistentRepresentation> recoveryCallback)
    {
        System.Console.WriteLine("Call - ReplayMessagesAsync");

        var highest = HighestSequenceNr(persistenceId);
        if (highest != 0L && max != 0L)
            Read(persistenceId, fromSequenceNr, Math.Min(toSequenceNr, highest), max).ForEach(recoveryCallback);
        return Task.FromResult(new object());
    }

    /// <summary>
    /// TBD
    /// </summary>
    /// <param name="persistenceId">TBD</param>
    /// <param name="toSequenceNr">TBD</param>
    /// <returns>TBD</returns>
    protected override Task DeleteMessagesToAsync(string persistenceId, long toSequenceNr)
    {
        System.Console.WriteLine("Call - DeleteMessagesToAsync");

        var highestSeqNr = HighestSequenceNr(persistenceId);
        var toSeqNr = Math.Min(toSequenceNr, highestSeqNr);
        if (toSeqNr == highestSeqNr)
            _meta.AddOrUpdate(persistenceId, highestSeqNr, (_, _) => highestSeqNr);
        for (var snr = 1L; snr <= toSeqNr; snr++)
            Delete(persistenceId, snr);
        return Task.FromResult(new object());
    }

    protected override bool ReceivePluginInternal(object message)
    {
        System.Console.WriteLine("Call - ReceivePluginInternal");

        switch (message)
        {
            case SelectCurrentPersistenceIds request:
                SelectAllPersistenceIdsAsync(request.Offset)
                    .PipeTo(request.ReplyTo, success: result => new CurrentPersistenceIds(result.Item1, result.LastOrdering));
                return true;
            
            case ReplayTaggedMessages replay:
                ReplayTaggedMessagesAsync(replay)
                    .PipeTo(replay.ReplyTo, success: h => new ReplayTaggedMessagesSuccess(h), failure: e => new ReplayMessagesFailure(e));
                return true;
            
            case ReplayAllEvents replay:
                ReplayAllEventsAsync(replay)
                    .PipeTo(replay.ReplyTo, success: h => new EventReplaySuccess(h),
                        failure: e => new EventReplayFailure(e));
                return true;
            
            default:
                return false;
        }
    }
    
    private Task<(IEnumerable<string> Ids, int LastOrdering)> SelectAllPersistenceIdsAsync(int offset)
    {
        System.Console.WriteLine("Call - SelectAllPersistenceIdsAsync");

        return Task.FromResult<(IEnumerable<string> Ids, int LastOrdering)>((new HashSet<string>(_allMessages.Skip(offset).Select(p => p.PersistenceId)), _allMessages.Count)); 
    }
    
    /// <summary>
    /// Replays all events with given tag withing provided boundaries from memory.
    /// </summary>
    /// <param name="replay">TBD</param>
    /// <returns>TBD</returns>
    private Task<int> ReplayTaggedMessagesAsync(ReplayTaggedMessages replay)
    {
        System.Console.WriteLine("Call - ReplayTaggedMessagesAsync");

        if (!_tagsToMessagesMapping.ContainsKey(replay.Tag))
            return Task.FromResult(0);

        int index = 0;
        foreach (var persistence in _tagsToMessagesMapping[replay.Tag]
                        .Skip(replay.FromOffset)
                        .Take(replay.ToOffset))
        {
            var payload = (Tagged)persistence.Payload;
            replay.ReplyTo.Tell(new ReplayedTaggedMessage(persistence.WithPayload(payload.Payload), replay.Tag, replay.FromOffset + index), ActorRefs.NoSender);
            index++;
        }

        return Task.FromResult(_tagsToMessagesMapping[replay.Tag].Count - 1);
    }
    
    private Task<int> ReplayAllEventsAsync(ReplayAllEvents replay)
    {
        System.Console.WriteLine("Call - ReplayAllEventsAsync");

        int index = 0;
        var replayed = _allMessages
            .Skip(replay.FromOffset)
            .Take(replay.ToOffset - replay.FromOffset)
            .ToArray();
        foreach (var message in replayed)
        {
            replay.ReplyTo.Tell(new ReplayedEvent(message, replay.FromOffset + index), ActorRefs.NoSender);
            index++;
        }
        return Task.FromResult(_allMessages.Count - 1);
    }
    
    #region QueryAPI

    [Serializable]
    public sealed class SelectCurrentPersistenceIds : IJournalRequest
    {
        public IActorRef ReplyTo { get; }
        public int Offset { get; }

        public SelectCurrentPersistenceIds(int offset, IActorRef replyTo)
        {
            Offset = offset;
            ReplyTo = replyTo;
        }
    }
    
    /// <summary>
    /// TBD
    /// </summary>
    [Serializable]
    public sealed class CurrentPersistenceIds : IDeadLetterSuppression
    {
        /// <summary>
        /// TBD
        /// </summary>
        public readonly IEnumerable<string> AllPersistenceIds;

        public readonly int HighestOrderingNumber;

        /// <summary>
        /// TBD
        /// </summary>
        /// <param name="allPersistenceIds">TBD</param>
        /// <param name="highestOrderingNumber">TBD</param>
        public CurrentPersistenceIds(IEnumerable<string> allPersistenceIds, int highestOrderingNumber)
        {
            AllPersistenceIds = allPersistenceIds.ToImmutableHashSet();
            HighestOrderingNumber = highestOrderingNumber;
        }
    }

    /// <summary>
    /// TBD
    /// </summary>
    [Serializable]
    public sealed class ReplayTaggedMessages : IJournalRequest
    {
        /// <summary>
        /// TBD
        /// </summary>
        public readonly int FromOffset;

        /// <summary>
        /// TBD
        /// </summary>
        public readonly int ToOffset;

        /// <summary>
        /// TBD
        /// </summary>
        public readonly int Max;

        /// <summary>
        /// TBD
        /// </summary>
        public readonly string Tag;

        /// <summary>
        /// TBD
        /// </summary>
        public readonly IActorRef ReplyTo;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReplayTaggedMessages"/> class.
        /// </summary>
        /// <param name="fromOffset">TBD</param>
        /// <param name="toOffset">TBD</param>
        /// <param name="max">TBD</param>
        /// <param name="tag">TBD</param>
        /// <param name="replyTo">TBD</param>
        /// <exception cref="ArgumentException">
        /// This exception is thrown for a number of reasons. These include the following:
        /// <ul>
        /// <li>The specified <paramref name="fromOffset"/> is less than zero.</li>
        /// <li>The specified <paramref name="toOffset"/> is less than or equal to zero.</li>
        /// <li>The specified <paramref name="max"/> is less than or equal to zero.</li>
        /// </ul>
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// This exception is thrown when the specified <paramref name="tag"/> is null or empty.
        /// </exception>
        public ReplayTaggedMessages(int fromOffset, int toOffset, int max, string tag, IActorRef replyTo)
        {
            System.Console.WriteLine("Call - ReplayTaggedMessages");

            if (fromOffset < 0)
                throw new ArgumentException("From offset may not be a negative number", nameof(fromOffset));
            if (toOffset <= 0) throw new ArgumentException("To offset must be a positive number", nameof(toOffset));
            if (max <= 0)
                throw new ArgumentException("Maximum number of replayed messages must be a positive number",
                    nameof(max));
            if (string.IsNullOrEmpty(tag))
                throw new ArgumentNullException(nameof(tag),
                    "Replay tagged messages require a tag value to be provided");

            FromOffset = fromOffset;
            ToOffset = toOffset;
            Max = max;
            Tag = tag;
            ReplyTo = replyTo;
        }
    }

    /// <summary>
    /// TBD
    /// </summary>
    [Serializable]
    public sealed class ReplayedTaggedMessage : INoSerializationVerificationNeeded, IDeadLetterSuppression
    {
        /// <summary>
        /// TBD
        /// </summary>
        public readonly IPersistentRepresentation Persistent;
        /// <summary>
        /// TBD
        /// </summary>
        public readonly string Tag;
        /// <summary>
        /// TBD
        /// </summary>
        public readonly int Offset;

        /// <summary>
        /// TBD
        /// </summary>
        /// <param name="persistent">TBD</param>
        /// <param name="tag">TBD</param>
        /// <param name="offset">TBD</param>
        public ReplayedTaggedMessage(IPersistentRepresentation persistent, string tag, int offset)
        {
            Persistent = persistent;
            Tag = tag;
            Offset = offset;
        }
    }
    
    /// <summary>
    /// TBD
    /// </summary>
    [Serializable]
    public sealed class ReplayAllEvents : IJournalRequest
    {
        /// <summary>
        /// TBD
        /// </summary>
        public readonly int FromOffset;
        /// <summary>
        /// TBD
        /// </summary>
        public readonly int ToOffset;
        /// <summary>
        /// TBD
        /// </summary>
        public readonly long Max;
        /// <summary>
        /// TBD
        /// </summary>
        public readonly IActorRef ReplyTo;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReplayAllEvents"/> class.
        /// </summary>
        /// <param name="fromOffset">TBD</param>
        /// <param name="toOffset">TBD</param>
        /// <param name="max">TBD</param>
        /// <param name="replyTo">TBD</param>
        /// <exception cref="ArgumentException">
        /// This exception is thrown for a number of reasons. These include the following:
        /// <ul>
        /// <li>The specified <paramref name="fromOffset"/> is less than zero.</li>
        /// <li>The specified <paramref name="toOffset"/> is less than or equal to zero.</li>
        /// <li>The specified <paramref name="max"/> is less than or equal to zero.</li>
        /// </ul>
        /// </exception>
        public ReplayAllEvents(int fromOffset, int toOffset, long max, IActorRef replyTo)
        {
            System.Console.WriteLine("Call - ReplayAllEvents");

            if (fromOffset < 0) throw new ArgumentException("From offset may not be a negative number", nameof(fromOffset));
            if (toOffset <= 0) throw new ArgumentException("To offset must be a positive number", nameof(toOffset));
            if (max <= 0) throw new ArgumentException("Maximum number of replayed messages must be a positive number", nameof(max));

            FromOffset = fromOffset;
            ToOffset = toOffset;
            Max = max;
            ReplyTo = replyTo;
        }
    }
    
    /// <summary>
    /// TBD
    /// </summary>
    [Serializable]
    public sealed class ReplayedEvent : INoSerializationVerificationNeeded, IDeadLetterSuppression
    {
        /// <summary>
        /// TBD
        /// </summary>
        public readonly IPersistentRepresentation Persistent;
        /// <summary>
        /// TBD
        /// </summary>
        public readonly int Offset;

        /// <summary>
        /// TBD
        /// </summary>
        /// <param name="persistent">TBD</param>
        /// <param name="offset">TBD</param>
        public ReplayedEvent(IPersistentRepresentation persistent, int offset)
        {
            Persistent = persistent;
            Offset = offset;
        }
    }

    /// <summary>
    /// TBD
    /// </summary>
    [Serializable]
    public sealed class ReplayTaggedMessagesSuccess
    {
        public ReplayTaggedMessagesSuccess(int highestSequenceNr)
        {
            HighestSequenceNr = highestSequenceNr;
        }

        /// <summary>
        /// Highest stored sequence number.
        /// </summary>
        public int HighestSequenceNr { get; }
    }
    
    /// <summary>
    /// TBD
    /// </summary>
    [Serializable]
    public sealed class EventReplaySuccess
    {
        public EventReplaySuccess(int highestSequenceNr)
        {
            HighestSequenceNr = highestSequenceNr;
        }

        /// <summary>
        /// Highest stored sequence number.
        /// </summary>
        public int HighestSequenceNr { get; }

        public bool Equals(EventReplaySuccess other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return Equals(HighestSequenceNr, other.HighestSequenceNr);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is EventReplaySuccess evt)) return false;
            return Equals(evt);
        }

        public override int GetHashCode() => HighestSequenceNr.GetHashCode();

        public override string ToString() => $"EventReplaySuccess<highestSequenceNr: {HighestSequenceNr}>";
    }

    public sealed class EventReplayFailure
    {
        public EventReplayFailure(Exception cause)
        {
            Cause = cause;
        }

        /// <summary>
        /// Highest stored sequence number.
        /// </summary>
        public Exception Cause { get; }

        public bool Equals(EventReplayFailure other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return Equals(Cause, other.Cause);
        }

    
        public override bool Equals(object obj)
        {
            if (!(obj is EventReplayFailure f)) return false;
            return Equals(f);
        }

    
        public override int GetHashCode() => Cause.GetHashCode();

    
        public override string ToString() => $"EventReplayFailure<cause: {Cause.Message}>";
    }

    #endregion
    
    #region IMemoryMessages implementation

    /// <summary>
    /// TBD
    /// </summary>
    /// <param name="persistent">TBD</param>
    /// <returns>TBD</returns>
    public Messages Add(IPersistentRepresentation persistent)
    {
        
        var list = Messages.GetOrAdd(persistent.PersistenceId, _ => new LinkedList<IPersistentRepresentation>());
        list.AddLast(persistent);
        return Messages;
    }

    /// <summary>
    /// TBD
    /// </summary>
    /// <param name="pid">TBD</param>
    /// <param name="seqNr">TBD</param>
    /// <param name="updater">TBD</param>
    /// <returns>TBD</returns>
    public Messages Update(string pid, long seqNr, Func<IPersistentRepresentation, IPersistentRepresentation> updater)
    {
        if (Messages.TryGetValue(pid, out LinkedList<IPersistentRepresentation> persistents))
        {
            var node = persistents.First;
            while (node != null)
            {
                if (node.Value.SequenceNr == seqNr)
                    node.Value = updater(node.Value);

                node = node.Next;
            }
        }

        return Messages;
    }

    /// <summary>
    /// TBD
    /// </summary>
    /// <param name="pid">TBD</param>
    /// <param name="seqNr">TBD</param>
    /// <returns>TBD</returns>
    public Messages Delete(string pid, long seqNr)
    {
        if (Messages.TryGetValue(pid, out LinkedList<IPersistentRepresentation> persistents))
        {
            var node = persistents.First;
            while (node != null)
            {
                if (node.Value.SequenceNr == seqNr)
                    persistents.Remove(node);

                node = node.Next;
            }
        }

        return Messages;
    }

    /// <summary>
    /// TBD
    /// </summary>
    /// <param name="pid">TBD</param>
    /// <param name="fromSeqNr">TBD</param>
    /// <param name="toSeqNr">TBD</param>
    /// <param name="max">TBD</param>
    /// <returns>TBD</returns>
    public IEnumerable<IPersistentRepresentation> Read(string pid, long fromSeqNr, long toSeqNr, long max)
    {
        if (Messages.TryGetValue(pid, out LinkedList<IPersistentRepresentation> persistents))
        {
            return persistents
                .Where(x => x.SequenceNr >= fromSeqNr && x.SequenceNr <= toSeqNr)
                .Take(max > int.MaxValue ? int.MaxValue : (int)max);
        }

        return Enumerable.Empty<IPersistentRepresentation>();
    }

    /// <summary>
    /// TBD
    /// </summary>
    /// <param name="pid">TBD</param>
    /// <returns>TBD</returns>
    public long HighestSequenceNr(string pid)
    {
        if (Messages.TryGetValue(pid, out LinkedList<IPersistentRepresentation> persistents))
        {
            var last = persistents.LastOrDefault();
            return last?.SequenceNr ?? 0L;
        }

        return 0L;
    }

    #endregion
}