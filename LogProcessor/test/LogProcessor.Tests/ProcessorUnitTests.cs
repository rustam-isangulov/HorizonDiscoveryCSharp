using Xunit;
using Xunit.Abstractions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LogProcessor;

namespace LogProcessor.Tests;

public class ProcessorUnitTests
{
    [Fact]
    public void EmptyProcessorEmptyFileList()
    {
        var content = new StringBuilder();
        var writer = new StringWriter(content);

        var processor = Processor.Builder
            .Processor()
            .WithAppender(writer.Write)
            .Build();

        processor.Process(Array.Empty<FileInfo>());

        Assert.Equal("", content.ToString());

    }
    [Fact]
    public void EmptyProcessorArbitraryContent()
    {
        var expectedString1 = "test1 content";
        var expectedString2 = "test2 content";
        var expected = new StringBuilder();
        var expectedWriter = new StringWriter( expected );
        expectedWriter.WriteLine( expectedString1 );
        expectedWriter.WriteLine( expectedString2 );

        using (var fileWriter1 = new StreamWriter(File.Create("test1.txt")))
        using (var fileWriter2 = new StreamWriter(File.Create("test2.txt")))
        {
            fileWriter1.WriteLine(expectedString1);
            fileWriter1.Flush();
            fileWriter2.WriteLine(expectedString2);
            fileWriter2.Flush();
        }


        var content = new StringBuilder();
        var writer = new StringWriter(content);

        var processor = Processor.Builder
            .Processor()
            .WithAppender(writer.WriteLine)
            .Build();

        processor.Process(new FileInfo[] 
        { 
            new FileInfo("test1.txt"), 
            new FileInfo("test2.txt") 
        });


        File.Delete("test1.txt");
        File.Delete("test2.txt");

        Assert.Equal(expected.ToString(), content.ToString());
    }
}
