using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace LogProcessor.Tests;

public class LogEntriesUnitTests : TestWithStandardOutput
{
    public LogEntriesUnitTests(ITestOutputHelper output) : base(output)
    {
    }

    [Fact]
    public void CreatingLogEntries()
    {
        var logEntries = LogEntries.Of(
            LogFields.Of(new List<string> { "zero", "one", "two" }),
            new List<IList<string>> {
                new List<string> { "a", "b", "c"},
                new List<string> { "a", "b", "c" }});

        Assert.Equal(new List<string> { "a", "b", "c" }, logEntries.Entries[0]);
        Assert.Equal("c", logEntries.Entries[1][logEntries.Fields["two"]]);
        Assert.Equal(
            new List<string> { "zero", "one", "two" },
            logEntries.Fields.Fields);
    }
}
