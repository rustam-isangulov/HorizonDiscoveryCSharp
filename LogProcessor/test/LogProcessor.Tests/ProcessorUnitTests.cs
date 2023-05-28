using Xunit;
using Xunit.Abstractions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LogProcessor;

namespace LogProcessor.Tests;

public class ProcessorUnitTests : TestWithStandardOutput
{
    private static readonly string _contentW3C = """
#Software: Microsoft HTTP Server API 2.0
#Version: 1.0   // the log file version as it's described by "https://www.w3.org/TR/WD-logfile".
#Date: 2002-05-02 17:42:15  // when the first log file entry was recorded, which is when the entire log file was created.
#Fields: date time c-ip cs-username s-ip s-port cs-method cs-uri-stem cs-uri-query sc-status cs(User-Agent)
2002-05-02 17:42:15 172.22.255.255 - 172.30.255.255 80 GET /images/picture.jpg - 200 Mozilla/4.0+(compatible;MSIE+5.5;+Windows+2000+Server)
#Remarks: comment string
2002-05-02 17:42:16 172.22.255.255 - 172.30.255.255 80 GET /images/picture.jpg - 200 Mozilla/4.0+(compatible;MSIE+5.5;+Windows+2000+Server)

2002-05-02 17:42:17 172.22.255.255 - 172.30.255.255 80 GET /images/picture.jpg - 200 Mozilla/4.0+(compatible;MSIE+5.5;+Windows+2000+Server)

""";

    private static readonly string _contentW3CParsed = """
2002-05-02 17:42:15 172.22.255.255 - 172.30.255.255 80 GET /images/picture.jpg - 200 Mozilla/4.0+(compatible;MSIE+5.5;+Windows+2000+Server)
2002-05-02 17:42:16 172.22.255.255 - 172.30.255.255 80 GET /images/picture.jpg - 200 Mozilla/4.0+(compatible;MSIE+5.5;+Windows+2000+Server)
2002-05-02 17:42:17 172.22.255.255 - 172.30.255.255 80 GET /images/picture.jpg - 200 Mozilla/4.0+(compatible;MSIE+5.5;+Windows+2000+Server)

""";

    private static readonly string _contentNCSA = """
172.22.255.255 - Microsoft\JohnDoe [02/May/2002:17:42:15 +0100] "GET /images/picture.jpg HTTP/1.0" 200 3256
172.22.255.255 - Microsoft\JohnDoe [02/May/2002:17:42:16 +0100] "GET /images/picture.jpg HTTP/1.0" 200 3256
172.22.255.255 - Microsoft\JohnDoe [02/May/2002:17:42:17 +0100] "GET /images/picture.jpg HTTP/1.0" 200 3256

""";

    private static readonly string _contentNCSAParsed = """
172.22.255.255 - Microsoft\JohnDoe 02/May/2002:17:42:15 +0100 GET /images/picture.jpg HTTP/1.0 200 3256
172.22.255.255 - Microsoft\JohnDoe 02/May/2002:17:42:16 +0100 GET /images/picture.jpg HTTP/1.0 200 3256
172.22.255.255 - Microsoft\JohnDoe 02/May/2002:17:42:17 +0100 GET /images/picture.jpg HTTP/1.0 200 3256

""";

    public ProcessorUnitTests(ITestOutputHelper output) : base(output)
    {
    }

    [Fact]
    public void ProcessingNCSALogs()
    {
        // arrange
        var tempFileName = CreateLogFile(_contentNCSA);

        var content = new StringBuilder();
        var writer = new StringWriter(content);

        // act
        Processor.Builder
            .Processor()
            .ForFormat(FormatProvider.GetNCSAFormatInfo())
            .WithAppender(writer.WriteLine)
            .Build()
            .Process(new FileInfo[] { new FileInfo(tempFileName) });

        // assert
        Assert.Equal(_contentNCSAParsed, content.ToString());

        // clean
        File.Delete(tempFileName);
    }
    
    [Fact]
    public void ProcessingW3CLogs()
    {
        // arrange
        var tempFileName = CreateLogFile(_contentW3C);

        var content = new StringBuilder();
        var writer = new StringWriter(content);

        // act
        Processor.Builder
            .Processor()
            .ForFormat(FormatProvider.GetW3CFormatInfo(tempFileName))
            .WithAppender(writer.WriteLine)
            .Build()
            .Process(new FileInfo[] { new FileInfo(tempFileName) });

        // assert
        Assert.Equal(_contentW3CParsed, content.ToString());

        // clean
        File.Delete(tempFileName);
    }

    private static string CreateLogFile(string content)
    {
        var tempFileName = Path.GetTempFileName();
        using (var fileWriter = new StreamWriter(File.Create(tempFileName)))
        {
            fileWriter.WriteLine(content);
            fileWriter.Flush();
        }

        return tempFileName;
    }

    [Fact]
    public void CreaingProcessorWithNoFormatInfo()
    {
        // arrange
        var content = new StringBuilder();
        var writer = new StringWriter(content);

        // act
        void buildProcessor() => Processor.Builder
            .Processor()
            .WithAppender(writer.Write)
            .Build();

        // assert
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(buildProcessor);
        Assert.Equal("[Processor::Builder::Build] FormatInfo is null!", exception.Message);
    }

    [Fact]
    public void EmptyProcessorEmptyFileList()
    {
        var content = new StringBuilder();
        var writer = new StringWriter(content);


        var processor = Processor.Builder
            .Processor()
            .ForFormat(FormatProvider.GetNCSAFormatInfo())
            .WithAppender(writer.Write)
            .Build();

        processor.Process(Array.Empty<FileInfo>());

        Assert.Equal("", content.ToString());

    }
}
