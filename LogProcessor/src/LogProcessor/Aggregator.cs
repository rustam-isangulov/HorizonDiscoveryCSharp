using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace LogProcessor;

public class Aggregator : ITransform
{
    private readonly IList<FieldAggregator> _aggregators;
    private readonly string _classifierSourceFieldName;
    private readonly string _classifierDestinationFieldName;
    private readonly string _counterDestinationFieldName;

    private Aggregator(IList<FieldAggregator> aggregators,
        string classifierSourceFieldName,
        string classifierDestinationFieldName,
        string counterDestinationFieldName)
    {
        _aggregators = aggregators;
        _counterDestinationFieldName = counterDestinationFieldName;
        _classifierSourceFieldName = classifierSourceFieldName;
        _classifierDestinationFieldName = classifierDestinationFieldName;
    }

    public LogEntries Apply(LogEntries source)
    {
        var query =
            from e in source.Entries
            group e by e[source.Fields[_classifierSourceFieldName]] into g
            select new
            {
                g.Key,
                Count = g.Count(),
                Aggregate = AggregateGroup(g.ToList(), source.Fields)
            };

        var aggregatedList = new List<IList<string>>();

        foreach (var e in query)
        {
            var newEntry = new List<string>()
                { e.Count.ToString(), e.Key };

            foreach (var agg in _aggregators)
            {
                newEntry.Add(e.Aggregate[source.Fields[agg.SourceFieldName]]);
            }

            aggregatedList.Add(newEntry);
        }

        var aggregatedLogFields = LogFields.Of(
            _aggregators
            .Select(a => a.DestinationFieldName)
            .Prepend(_classifierDestinationFieldName)
            .Prepend(_counterDestinationFieldName)
            .ToList());

        return LogEntries.Of(aggregatedLogFields, aggregatedList);
    }

    private IList<string> AggregateGroup(IList<IList<string>> g, LogFields logFields)
    {
        var result = new List<string>(g.First());

        foreach (var e in g.Skip(1))
        {
            foreach (var agg in _aggregators)
            {
                int index = logFields[agg.SourceFieldName];
                result[index] = agg.AggragatorFunc
                    (result[index], e[index]);
            }
        }

        return result;
    }

    public class Builder
    {
        private string _classifierSourceFieldName = "";
        private string _classifierDestinationFieldName = "";
        private string _counterDestinationFieldName = "";
        private readonly IList<FieldAggregator> _aggregators = new List<FieldAggregator>();

        public static Builder Aggregator()
        {
            return new Builder();
        }

        public Builder WithClassifier
            (string sourceFieldName, string destinationFieldName)
        {
            _classifierSourceFieldName = sourceFieldName;
            _classifierDestinationFieldName = destinationFieldName;

            return this;
        }

        public Builder WithCounter(string destinationCounterFieldName)
        {
            _counterDestinationFieldName = destinationCounterFieldName;

            return this;
        }

        public Builder WithFieldAggregator(
            string sourceFieldName,
            string destinationFieldName,
            Func<string, string, string> aggregatorFunc)
        {
            _aggregators.Add(
                new(sourceFieldName, destinationFieldName, aggregatorFunc));

            return this;
        }

        public Aggregator Build()
        {
            if (string.IsNullOrEmpty(_classifierSourceFieldName) ||
                string.IsNullOrEmpty(_classifierDestinationFieldName))
            {
                throw new InvalidOperationException("[Aggregator::Builder::Build] a classifier field name is empty!");
            }

            if (string.IsNullOrEmpty(_counterDestinationFieldName))
            {
                throw new InvalidOperationException("[Aggregator::Builder::Build] the counter field name is empty!");
            }

            var diffChecker  = new HashSet<string>();
            if ( !(diffChecker.Add(_counterDestinationFieldName) && 
                diffChecker.Add(_classifierDestinationFieldName) &&
                _aggregators.All(agg => diffChecker.Add(agg.DestinationFieldName))) )
            {
                throw new ArgumentException
                    ("[Aggregator::Builder::Build] List of destination field names has non-unique elements!");
            }

            return new Aggregator(
                _aggregators, 
                _classifierSourceFieldName, 
                _classifierDestinationFieldName, 
                _counterDestinationFieldName);
        }
    }
}

internal record FieldAggregator(
    string SourceFieldName,
    string DestinationFieldName,
    Func<string, string, string> AggragatorFunc);

