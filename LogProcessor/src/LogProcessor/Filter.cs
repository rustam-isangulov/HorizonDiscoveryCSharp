using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LogProcessor;

public class Filter : ITransform
{
    private readonly IDictionary<string, string> _filters;

    private Filter(IDictionary<string, string> filters)
    {
        _filters = filters;
    }

    public static Filter Of(IDictionary<string, string> filters)
    {
        return new Filter(filters);
    }

    public LogEntries Apply(LogEntries source)
    {
        if (!_filters.Keys.All(source.Fields.Fields.Contains))
        {
            throw new ArgumentException
                ("[Filter::Apply] A filter key is not matching any source fields!");
        }

        var resultEntries = source.Entries.Where(
            e => _filters.All(
                f => Regex.IsMatch(e[source.Fields[f.Key]], f.Value)));

        return LogEntries.Of(source.Fields, resultEntries.ToList());
    }
}
