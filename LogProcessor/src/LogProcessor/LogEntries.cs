using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogProcessor;

public class LogEntries
{
    private LogEntries(LogFields fields, IList<IList<string>> entries)
    {
        Fields = fields;
        Entries = entries;
    }

    public static LogEntries Of(LogFields fields, IList<IList<string>> entries)
    {
        return new LogEntries(LogFields.Of(fields.Fields), entries);
    }

    public IList<IList<string>> Entries { get; }
    public LogFields Fields { get; }
}
