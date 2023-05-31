using LogProcessor.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace LogProcessor.Tests;

public class SorterUnitTests : TestWithStandardOutput
{
    public SorterUnitTests(ITestOutputHelper output) : base(output)
    {
    }

    [Fact]
    public void SortingLogEntriesDescending()
    {
        // arrange
        var logEntries = LogEntries.Of(
            LogFields.Of(new List<string> { "zero", "one", "two" }),
            new List<IList<string>> {
                new List<string> { "a", "b", "c"},
                new List<string> { "d", "e", "f" },
                new List<string> { "404", "e", "g" },
                new List<string> { "403", "e", "f" },
                new List<string> { "504", "e", "f" }
            });

        var sorter = Sorter.Of("zero");
        
        // act
        var sortedEntries = sorter.Apply(logEntries);

        // assert
        Assert.Equal(new List<string> { "d", "e", "f" }, sortedEntries.Entries[0]);
        Assert.Equal(new List<string> { "403", "e", "f" }, sortedEntries.Entries.Last());
    }


    [Fact]
    public void SortingLogEntriesAscendingAsInts()
    {
        // arrange
        var logEntries = LogEntries.Of(
            LogFields.Of(new List<string> { "zero", "one", "two" }),
            new List<IList<string>> {
                new List<string> { "41", "b", "c"},
                new List<string> { "405", "e", "f" },
                new List<string> { "404", "e", "g" },
                new List<string> { "403", "e", "f" },
                new List<string> { "504", "e", "f" }
            });

        var sorter = Sorter.Of("zero", new CompareAsInts(), descending: false);
        
        // act
        var sortedEntries = sorter.Apply(logEntries);

        // assert
        Assert.Equal(new List<string> { "41", "b", "c" }, sortedEntries.Entries[0]);
        Assert.Equal(new List<string> { "504", "e", "f" }, sortedEntries.Entries[4]);
    }

    [Fact]
    public void SortingByNonExistingField()
    {
        // arrange
        var logEntries = LogEntries.Of(
            LogFields.Of(new List<string> { "zero", "one", "two" }),
            new List<IList<string>> {
                new List<string> { "a", "b", "c"},
                new List<string> { "d", "e", "f" },
                new List<string> { "404", "e", "g" },
                new List<string> { "403", "e", "f" },
                new List<string> { "504", "e", "f" }
            });

        var sorter = Sorter.Of("NON-EXISITNG-FIELD");

        // act
        void apply() => sorter.Apply(logEntries);

        // assert
        ArgumentException exception = Assert.Throws<ArgumentException>(apply);
        Assert.Equal("[Sorter::Apply] The sorter key [NON-EXISITNG-FIELD] is not matching any source fields!", exception.Message);
    }

}
