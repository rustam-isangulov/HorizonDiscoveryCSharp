using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogProcessor
{
    public class Processor : IProcessor
    {
        private readonly Action<string> _appender;
        private readonly IFormatInfo _formatInfo;
        private readonly Queue<ITransform> _transforms;

        private Processor(Action<string> appender, IFormatInfo formatInfo, Queue<ITransform> transforms)
        {
            _appender = appender;
            _formatInfo = formatInfo;
            _transforms = new Queue<ITransform>(transforms);
        }

        public void Process(IList<FileInfo> files)
        {
            Load(Transform(Extract(files)));
        }

        private void Load(LogEntries processedEntries)
        {
            foreach (var entry in processedEntries.Entries) 
            {
                _appender(String.Join(" ", entry.ToArray()));
            }
        }

        private LogEntries Transform(LogEntries sourceEntries) 
        {
            LogEntries result = sourceEntries;

            foreach (var transform in _transforms)
            {
                result = transform.Apply(result);
            }

            return result;
        }

        private LogEntries Extract(IList<FileInfo> files)
        {
            var lineQuery =
                from file in files
                from line in File.ReadLines(file.FullName)
                where _formatInfo.IsValidEntry(line)
                select _formatInfo.Parser(line);

            var expectedNumberOfValuesPerEntry = _formatInfo.Fields.Count;
            
            var entriesWithData = lineQuery.ToList();
            entriesWithData.RemoveAll( e => e.Count != expectedNumberOfValuesPerEntry );

            return LogEntries.Of(
                LogFields.Of(_formatInfo.Fields), 
                entriesWithData);
        }

        public class Builder
        {
            private Queue<ITransform> _transforms = new Queue<ITransform>();
            private Action<string> _appender = Console.WriteLine;
            private IFormatInfo? _format;

            public static Builder Processor() 
            {
                return new Builder();
            }

            public Builder ParseAs(IFormatInfo formatInfo) 
            {
                _format = formatInfo;
                return this;
            }

            public Builder ThenFilter(ITransform filter) 
            { 
                _transforms.Enqueue(filter);
                return this;
            }

            public Builder ThenAggregator(ITransform aggregator) 
            { 
                _transforms.Enqueue(aggregator);
                return this;
            }

            public Builder ThenSort(ITransform sorter) 
            { 
                _transforms.Enqueue(sorter);
                return this;
            }

            public Builder WithAppender( Action<string> appender)
            {
                _appender = appender;
                return this;
            }

            public Processor Build()
            {
                if (_format == null)
                {
                    throw new InvalidOperationException("[Processor::Builder::Build] FormatInfo is null!");
                }

                return new Processor(_appender, _format, _transforms);
            }
        }
    }
}
