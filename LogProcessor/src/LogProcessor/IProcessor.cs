using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogProcessor
{
    internal interface IProcessor
    {
        void Process(IList<FileInfo> files);
    }
}
