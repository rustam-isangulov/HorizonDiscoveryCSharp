using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace LogProcessor.Tests;

public class FilterUnitTests : TestWithStandardOutput
{
    public FilterUnitTests(ITestOutputHelper output) : base(output)
    {
    }

    [Fact]
    public void FilteringLogEntries()
    {
        // arrange
        var logEntries = LogEntries.Of(
            LogFields.Of(new List<string> { "zero", "one", "two" }),
            new List<IList<string>> {
                new List<string> { "a", "b", "c"},
                new List<string> { "d", "e", "f" },
                new List<string> { "404", "e", "g" },
                new List<string> { "404", "e", "f" },
                new List<string> { "504", "e", "f" }
            });

        var filter = Filter.Of(new Dictionary<string, string>() 
        { 
            { "two", "f" },
            { "zero", ".+4" }
        });

        // act
        var filteredEntries = filter.Apply(logEntries);

        // assert
        Assert.Equal(new List<string> { "404", "e", "f" }, filteredEntries.Entries[0]);
        Assert.Equal(new List<string> { "504", "e", "f" }, filteredEntries.Entries[1]);
    }

    [Fact]
    public void ApplyingFilterWithFieldsMismatch()
    {
        // arrange
        var logEntries = LogEntries.Of(
            LogFields.Of(new List<string> { "zero", "one", "two" }),
            new List<IList<string>> {
                new List<string> { "a", "b", "c"},
                new List<string> { "d", "e", "f" },
                new List<string> { "404", "e", "g" },
                new List<string> { "404", "e", "f" },
                new List<string> { "504", "e", "f" }
            });

        var filter = Filter.Of(new Dictionary<string, string>()
        {
            { "BAD-FIELD-NAME", "f" },
            { "zero", ".+4" }
        });

        // act
        void apply() => filter.Apply(logEntries);

        // assert
        ArgumentException exception = Assert.Throws<ArgumentException>(apply);
        Assert.Equal("[Filter::Apply] A filter key is not matching any source fields!", exception.Message);
    }
}
