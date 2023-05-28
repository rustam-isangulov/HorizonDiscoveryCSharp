using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LogProcessor
{
    public class FormatProvider
    {
        public static IFormatInfo GetW3CFormatInfo(string fileName)
        {
            var queryFields =
                from line in File.ReadLines(fileName)
                where line.StartsWith("#Fields:")
                select line.TrimStart("#Fields:".ToCharArray()).Trim().Split(" ");

            var fields = queryFields.First().ToList();

            return new FormatInfo(
                fields: new List<string>(fields),
                entryPredicate: (entry) => !string.IsNullOrEmpty(entry) && !entry.StartsWith("#"),
                parser: (entry) => entry.Split(" "));
        }

        public static IFormatInfo GetNCSAFormatInfo()
        {
            var fields = new List<string>()
            {
                "remote_host_address",
                "remote_log_name",
                "user_name",
                "date_time",
                "method",
                 "uri",
                "protocol",
                "status_code",
                "bytes_sent"
            };

            return new FormatInfo(
                fields: new List<string>(fields),
                entryPredicate: (entry) => true,
                parser: (entry) =>
                {
                    string pattern = "(\\S+) (\\S+) (\\S+) \\[([\\w:/]+\\s[+\\-]\\d{4})\\] \\\"(\\S+) (\\S+)\\s*(\\S+)?\\s*\\\" (\\d{3}) (\\S+)";
                    Match match = Regex.Match(entry, pattern);

                    if (match.Success) 
                    {
                        var query = 
                            from g in match.Groups.Values
                            select g.Value;

                        return query.Skip(1).ToList();
                    }

                    return Array.Empty<string>();
                });
        }
    }
}
