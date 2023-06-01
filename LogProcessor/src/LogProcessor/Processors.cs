using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogProcessor;

public class Processors
{
    public static IProcessor GetW3CProcessor(IList<FileInfo> files, Action<string> writer)
    {
        return Processor.Builder
           .Processor()
           .ParseAs(FormatProvider.GetW3CFormatInfo(files.First().FullName))
           .ThenAggregator
               (Aggregator.Builder.Aggregator()
               .WithClassifier("cs-uri-stem", "cs-uri-stem-classfier")
               .WithCounter("uri-counter")
               .WithFieldAggregator("date", "date", (prev, curr) => prev.CompareTo(curr) < 0 ? curr : prev)
               .WithFieldAggregator("time", "time", (prev, curr) => prev.CompareTo(curr) < 0 ? curr : prev)
               .Build())
            .ThenSort(Sorter.Of("uri-counter", Sorter.IntComparer))
           .WithAppender(writer)
           .Build();
    }    
    
    public static IProcessor GetNCSAProcessor(Action<string> writer)
    {
        return Processor.Builder
           .Processor()
           .ParseAs(FormatProvider.GetNCSAFormatInfo())
           .ThenAggregator
               (Aggregator.Builder.Aggregator()
               .WithClassifier("uri", "uri-classfier")
               .WithCounter("uri-counter")
               .WithFieldAggregator("date_time", "date_time", (prev, curr) => prev.CompareTo(curr) < 0 ? curr : prev)
               .WithFieldAggregator("bytes_sent", "bytes_sent", (prev, curr) => prev.CompareTo(curr) < 0 ? curr : prev)
               .Build())
           .ThenSort(Sorter.Of("uri-counter", Sorter.IntComparer))
           .WithAppender(writer)
           .Build();
    }
}
