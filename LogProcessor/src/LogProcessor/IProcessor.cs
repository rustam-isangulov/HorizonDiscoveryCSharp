using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogProcessor;

public interface IProcessor
{
    void Process(IList<FileInfo> files);
}
