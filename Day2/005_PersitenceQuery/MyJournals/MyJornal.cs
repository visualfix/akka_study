//-----------------------------------------------------------------------
// <copyright file="DummyReadJournal.cs" company="Akka.NET Project">
//     Copyright (C) 2009-2023 Lightbend Inc. <http://www.lightbend.com>
//     Copyright (C) 2013-2023 .NET Foundation <https://github.com/akkadotnet/akka.net>
// </copyright>
//-----------------------------------------------------------------------

using Akka;
using System.Collections.Generic;
using Akka.Configuration;
using Akka.Streams.Dsl;
using Akka.Persistence.Query;

namespace FSM004.MyJournals
{
    /// <summary>
    /// Use for tests only!
    /// Emits infinite stream of strings (representing queried for events).
    /// </summary>
    public class MyJournal : IPersistenceIdsQuery
    {
        public static readonly string Identifier = "akka.persistence.query.my-journal";

        public Source<string, NotUsed> PersistenceIds() => Source.From(Iterate(0, 10)).Select(i => i.ToString());

        private IEnumerable<int> Iterate(int start, int end)
        {
            while (start < end) yield return start++;
        }
    }

    public class MyJournalProvider : IReadJournalProvider
    {
        public static Config Config => ConfigurationFactory.ParseString(
            $@"{MyJournal.Identifier} {{ class = ""{typeof (MyJournalProvider).FullName}, FSM004"" }}");

        public IReadJournal GetReadJournal()
        {
            return new MyJournal();
        }
    }
}