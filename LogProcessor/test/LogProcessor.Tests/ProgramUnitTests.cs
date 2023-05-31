using Xunit;
using Xunit.Abstractions;

using LogProcessor;

namespace LogProcessor.Tests;

public class ProgramUnitTests : TestWithStandardOutput
{
    public ProgramUnitTests(ITestOutputHelper output) : base(output)
    {
    }

    [Fact]
    public void EmptyArgumentsString()
    {
        var expected = 1;

        var actual = Program.Main(Array.Empty<string>());

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void GoodArgumentsString()
    {
        var tempFileName = Path.GetTempFileName();

        File.Create(tempFileName).Close();

        var expected = 0;

        var actual = Program.Main(new string[]
        {
            "--files", tempFileName,
            "--type", "NCSA"
        });

        Assert.Equal(expected, actual);
        
        File.Delete(tempFileName);
    }
}
