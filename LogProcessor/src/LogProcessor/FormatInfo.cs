using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogProcessor
{
    internal class FormatInfo : IFormatInfo
    {
        private readonly IList<string> _fields;
        private readonly Predicate<string> _entryPredicate;
        private readonly Func<string, IList<string>> _parser;

        public FormatInfo (IList<string> fields, Predicate<string> entryPredicate, Func<string, IList<string>> parser)
        {
            _fields = new List<string>(fields);
            _entryPredicate = entryPredicate;
            _parser = parser;
        }

        public IList<string> Fields => _fields;

        public Predicate<string> IsValidEntry => _entryPredicate;

        public Func<string, IList<string>> Parser => _parser;
    }
}
