using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace LogProcessor.Tests
{
    public class TestWithStandardOutput : IDisposable
    {
        private readonly ITestOutputHelper _output;
        private readonly StringWriter _writer = new();
        public TestWithStandardOutput(ITestOutputHelper output)
        {
            _output = output;
            Console.SetOut(_writer);
        }
        public void Dispose()
        {

            _output.WriteLine(_writer.ToString());
            GC.SuppressFinalize(this);
        }
    }
}
