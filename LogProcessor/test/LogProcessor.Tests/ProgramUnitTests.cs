using Xunit;
using Xunit.Abstractions;

using LogProcessor;

namespace LogProcessor.Tests;

public class ProgramUnitTests
{
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
        var expected = 0;
        var actual = -1;

        using (File.Create("test.txt"))
        {
            actual = Program.Main(new string[]
            {
            "--files", "test.txt",
            "--type", "W3C"
            });
        }

        File.Delete("test.txt");

        Assert.Equal(expected, actual);
    }
}
