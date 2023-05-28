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
        private Processor(Action<string> appender, IFormatInfo formatInfo ) 
        {
            _appender = appender;
            _formatInfo = formatInfo;
        }

        public void Process(IList<FileInfo> files)
        {
            Load(Transform(Extract(files)));
        }

        private void Load(IList<IList<string>> processedEntries)
        {
            foreach (var entry in processedEntries) 
            {
                _appender(String.Join(" ", entry.ToArray()));
            }
        }

        private IList<IList<string>> Transform(IList<IList<string>> sourceEntries) 
        {
            return sourceEntries;
        }

        private IList<IList<string>> Extract(IList<FileInfo> files)
        {
            var lineQuery =
                from file in files
                from line in File.ReadLines(file.FullName)
                where _formatInfo.IsValidEntry(line)
                select _formatInfo.Parser(line);

            var expectedNumberOfValuesPerEntry = _formatInfo.Fields.Count;
            
            var entriesWithData = lineQuery.ToList();
            entriesWithData.RemoveAll( e => e.Count != expectedNumberOfValuesPerEntry );

            return entriesWithData;
        }

        public class Builder
        {
            private Action<string> _appender = Console.WriteLine;
            private IFormatInfo? _format;

            public static Builder Processor() 
            {
                return new Builder();
            }

            public Builder ForFormat(IFormatInfo formatInfo) 
            {
                _format = formatInfo;
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

                return new Processor(_appender, _format);
            }
        }
    }
}
