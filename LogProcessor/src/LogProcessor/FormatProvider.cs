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
        private static readonly string _w3cFieldsSpecifier = "#Fields:";

        public static IFormatInfo GetW3CFormatInfo(string fileName)
        {
            var queryFields =
                from line in File.ReadLines(fileName)
                where line.StartsWith(_w3cFieldsSpecifier)
                select line.TrimStart(_w3cFieldsSpecifier.ToCharArray()).Trim().Split(" ", StringSplitOptions.RemoveEmptyEntries);

            if (!queryFields.Any())
            {
                throw new Exception($"[FormatProvider::GetW3CFormatInfo][ERROR]: {_w3cFieldsSpecifier} specifier is missing or malformed in [{fileName}]!");
            }

            var fields = queryFields.First().ToList();

            if (fields.Count == 0)
            {
                throw new Exception($"[FormatProvider::GetW3CFormatInfo][ERROR]: {_w3cFieldsSpecifier} specifier defines no fields in [{fileName}]!");
            }

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

                    // returning empty string instead of
                    // an /unrecognized/ entry structure
                    // maybe a problem for post processing
                    // consider filtering empty Lists as the next step
                    return Array.Empty<string>();
                });
        }
    }
}
