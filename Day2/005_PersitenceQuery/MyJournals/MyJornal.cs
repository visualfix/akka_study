//-----------------------------------------------------------------------
// <copyright file="DummyReadJournal.cs" company="Akka.NET Project">
//     Copyright (C) 2009-2023 Lightbend Inc. <http://www.lightbend.com>
//     Copyright (C) 2013-2023 .NET Foundation <https://github.com/akkadotnet/akka.net>
// </copyright>
//-----------------------------------------------------------------------

using Akka;
using Akka.Configuration;
using Akka.Streams.Dsl;
using Akka.Persistence.Query;

namespace PersitenceQuery.MyJournals;

 public class MyJournal : IPersistenceIdsQuery
{
    public static readonly string Identifier = "akka.persistence.query.journal.my-journal";

    public Source<string, NotUsed> PersistenceIds() => Source.From(Iterate(0, 10)).Select(i => i.ToString());

    private IEnumerable<int> Iterate(int start, int end)
    {
        int s = start;
        while (s < end) yield return s++;
        Thread.Sleep(3000);

        s = start;
        while (s < end) yield return s++;
    }

}

public class MyJournalProvider : IReadJournalProvider
{
    public static Config Config => ConfigurationFactory.ParseString(
        $@"{MyJournal.Identifier} {{ class = ""{typeof (MyJournalProvider).FullName}, PersitenceQuery"" }}");

    public IReadJournal GetReadJournal()
    {
        return new MyJournal();
    }
}