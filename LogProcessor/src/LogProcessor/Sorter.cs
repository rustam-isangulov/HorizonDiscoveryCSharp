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
    private readonly Comparer<string> _sorterComparer;

    private Sorter(string sortingFeildName, Comparer<string> comparer, bool descending)
    {
        _sorter = sortingFeildName;
        _descending = descending;
        _sorterComparer = comparer;
    }

    public static Sorter Of(
        string sorter, 
        bool descending = true)
    {
        return new Sorter(sorter, StringComparer, descending);
    }

    public static Sorter Of(
        string sorter,
        Comparer<string> comparer,
        bool descending = true)
    {
        return new Sorter(sorter, comparer, descending);
    }

    public static Comparer<string> StringComparer => new CompareAsStrings();
    public static Comparer<string> IntComparer => new CompareAsInts();

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
            .OrderByDescending(e => e[source.Fields[_sorter]], _sorterComparer)
            .ToList()
            :
            source.Entries
            .OrderBy(e => e[source.Fields[_sorter]], _sorterComparer)
            .ToList();

        return LogEntries.Of(
            source.Fields,
            orderedEntries());
    }
}

public class CompareAsStrings : Comparer<string>
{
    public override int Compare(string? x, string? y)
    {
        return string.Compare(x, y);
    }
}

public class CompareAsInts : Comparer<string>
{
    public override int Compare(string? x, string? y)
    {
        if (x == null || y == null)
        {
            return string.Compare(x, y);
        }

        return Int32.Parse(x) - Int32.Parse(y);
    }
}
