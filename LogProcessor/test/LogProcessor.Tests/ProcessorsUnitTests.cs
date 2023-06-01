using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogProcessor.Tests;

public class ProcessorsUnitTests
{
    private static readonly string contentW3C = """
#Software: Microsoft HTTP Server API 2.0
#Version: 1.0   // the log file version as it's described by "https://www.w3.org/TR/WD-logfile".
#Date: 2002-05-02 17:42:15  // when the first log file entry was recorded, which is when the entire log file was created.
#Fields: date time c-ip cs-username s-ip s-port cs-method cs-uri-stem cs-uri-query sc-status cs(User-Agent)
2002-05-02 17:42:15 172.22.255.255 - 172.30.255.255 80 GET /images/picture.jpg - 200 Mozilla/4.0+(compatible;MSIE+5.5;+Windows+2000+Server)
#Remarks: comment string
2002-05-02 17:42:16 172.22.255.255 - 172.30.255.255 80 GET /images/picture.jpg - 200 Mozilla/4.0+(compatible;MSIE+5.5;+Windows+2000+Server)

2002-05-02 17:42:17 172.22.255.255 - 172.30.255.255 80 GET /images/picture.jpg - 200 Mozilla/4.0+(compatible;MSIE+5.5;+Windows+2000+Server)
""";

    private static readonly string contentNCSA = """
172.22.255.255 - Microsoft\JohnDoe [02/May/2002:17:42:15 +0100] "GET /images/picture.jpg HTTP/1.0" 200 3256
172.22.255.255 - Microsoft\JohnDoe [02/May/2002:17:42:16 +0100] "GET /images/picture.jpg HTTP/1.0" 200 3256
172.22.255.255 - Microsoft\JohnDoe [02/May/2002:17:42:17 +0100] "GET /images/picture.jpg HTTP/1.0" 200 3256
""";

    [Fact]
    public void GetW3CProcessorTest()
    {
        // arrange
        var tempFileName = CreateW3CLogFile(contentW3C);

        // act
        var processor = Processors.GetW3CProcessor(new[] {new FileInfo(tempFileName)}, Console.WriteLine);

        // assert
        Assert.IsAssignableFrom<IProcessor>(processor);

        // clean
        File.Delete(tempFileName);
    }

    [Fact]
    public void GetNCSAProcessorTest()
    {
        // arrange
        var tempFileName = CreateW3CLogFile(contentNCSA);

        // act
        var processor = Processors.GetNCSAProcessor(Console.WriteLine);

        // assert
        Assert.IsAssignableFrom<IProcessor>(processor);

        // clean
        File.Delete(tempFileName);
    }

    private static string CreateW3CLogFile(string content)
    {
        var tempFileName = Path.GetTempFileName();
        using (var fileWriter = new StreamWriter(File.Create(tempFileName)))
        {
            fileWriter.WriteLine(content);
            fileWriter.Flush();
        }

        return tempFileName;
    }
}
