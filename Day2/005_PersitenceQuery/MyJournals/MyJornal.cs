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

    public Source<string, NotUsed> PersistenceIds() => Source.From(Iterate(0, 5)).Select(i => i.ToString());

    private IEnumerable<string> Iterate(int start, int end)
    {
        while (start < end) 
        {
            Thread.Sleep(500);
            yield return $"{start++} - {DateTime.Now.ToString()}";
        }
        
        Thread.Sleep(3000);
        while (start < (end * 2))
        {
            Thread.Sleep(500);
            yield return $"{start++} - {DateTime.Now.ToString()}";
        } 
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