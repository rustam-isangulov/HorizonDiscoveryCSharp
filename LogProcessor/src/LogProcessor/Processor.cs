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
        private Processor(Action<string> appender ) 
        {
            _appender = appender;
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
                select (IList<string>)line.Split(" ");

            return lineQuery.ToList();
        }

        public class Builder
        {
            private Action<string> _appender = Console.WriteLine;

            public static Builder Processor() 
            {
                return new Builder();
            }

            public Builder WithAppender( Action<string> appender)
            {
                _appender = appender;
                return this;
            }

            public Processor Build()
            {
                return new Processor(_appender);
            }
        }
    }
}
