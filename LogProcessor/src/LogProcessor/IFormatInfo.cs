using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogProcessor
{
    public interface IFormatInfo
    {
        IList<string> Fields { get; }
        Predicate<string> IsValidEntry { get; }
        Func<string, IList<string>> Parser { get; }
    }
}
