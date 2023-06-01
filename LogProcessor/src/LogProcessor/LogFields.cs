using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace LogProcessor;

public class LogFields
{
    private readonly IList<string> _fields;
    private readonly IDictionary<string, int> _fieldToIndexMap;

    private LogFields(IList<string> fields)
    {
        _fields = fields;

        int index = 0;
       _fieldToIndexMap = _fields
            .Select(fieldName => new { index = index++, fieldName })
            .ToDictionary(e => e.fieldName, e => e.index);
    }

    public static LogFields Of( IList<string> fields ) 
    {
        var diffChecker = new HashSet<string>();
        bool allAreDifferent = fields.All(diffChecker.Add);

        if (!allAreDifferent ) 
        {
            throw new ArgumentException
                ("[LogFields::Of][ERROR] List of field names has non-unique elements!");
        }

        return new LogFields( fields );
    }

    public IList<string> Fields => _fields;

    public int this[string fieldName] => _fieldToIndexMap[fieldName];
}
