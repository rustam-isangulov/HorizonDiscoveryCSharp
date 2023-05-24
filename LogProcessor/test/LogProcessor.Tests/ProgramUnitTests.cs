using Xunit;
using Xunit.Abstractions;

using LogProcessor;

namespace LogProcessor.Tests;

public class ProgramUnitTests
{
    [Fact]
    public void Test_EmptyArgumentsString()
    {
	var expected = 1;

	var actual = Program.Main(new string[0]);

	Assert.Equal(expected, actual);
    }

    [Fact]
    public void Test_GoodArgumentsString()
    {
	var expected = 0;

	var actual = Program.Main
	    (
		new string[]
		{
		    "--files", "one.txt", "two.txt",
		    "--type", "W3C"
		}
	    );

	Assert.Equal(expected, actual);
    }
}
