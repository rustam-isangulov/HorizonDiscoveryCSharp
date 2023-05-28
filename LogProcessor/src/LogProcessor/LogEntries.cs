using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogProcessor;

public class LogEntries
{
    private readonly IList<IList<string>> _entries;

    private LogEntries(LogFields fields, IList<IList<string>> entries)
    {
        Fields = fields;
        _entries = entries;
    }

    public static LogEntries Of(LogFields fields, IList<IList<string>> entries)
    {
        return new LogEntries(fields, entries);
    }

    public IList<IList<string>> Entries => _entries;
    public LogFields Fields { get; }
}
