using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace LogProcessor.Tests;


public class AggregateUnitTests : TestWithStandardOutput
{
    public AggregateUnitTests(ITestOutputHelper output) : base(output)
    {
    }

    [Fact]
    public void BasicAggregate()
    {
        // arrange
        var logEntries = LogEntries.Of(
            LogFields.Of(new List<string> { "zero", "one", "two" }),
            new List<IList<string>> {
                new List<string> { "200", "a", "b"},
                new List<string> { "200", "b", "c"},
                new List<string> { "200", "e", "f" },
                new List<string> { "404", "e", "g" },
                new List<string> { "404", "e", "f" },
                new List<string> { "502", "e", "f" }
            });


        // act
        var aggregatedEntries = Aggregator.Builder
            .Aggregator()
            .WithClassifier("zero", "zero_as_classifier")
            .WithCounter("zero_classifier_counter")
            .WithFieldAggregator("one", "one_concat", (prev, curr) => prev + "+" + curr)
            .Build()
            .Apply(logEntries);

        // assert
        Assert.Equal(3, aggregatedEntries.Entries.Count);
        Assert.Equal(3, int.Parse
            (aggregatedEntries.Entries[0][aggregatedEntries.Fields["zero_classifier_counter"]]));
    }
}
