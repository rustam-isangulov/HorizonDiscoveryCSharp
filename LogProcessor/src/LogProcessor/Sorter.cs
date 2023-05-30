using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogProcessor;

public class Sorter : ITransform
{
    private readonly string _sorter;
    private readonly bool _descending;

    private Sorter(string sortingFeildName, bool descending)
    {
        _sorter = sortingFeildName;
        _descending = descending;
    }

    public static Sorter Of(string sorter, bool descending = true)
    {
        return new Sorter(sorter, descending);
    }

    public LogEntries Apply(LogEntries source)
    {
        if (!source.Fields.Fields.Contains(_sorter))
        {
            throw new ArgumentException
                ($"[Sorter::Apply] The sorter key [{_sorter}] is not matching any source fields!");
        }

        IList<IList<string>> orderedEntries() => 
            _descending
            ? 
            source.Entries
            .OrderByDescending(e => e[source.Fields[_sorter]])
            .ToList()
            :
            source.Entries
            .OrderBy(e => e[source.Fields[_sorter]])
            .ToList();

        return LogEntries.Of(
            source.Fields,
            orderedEntries());
    }
}
