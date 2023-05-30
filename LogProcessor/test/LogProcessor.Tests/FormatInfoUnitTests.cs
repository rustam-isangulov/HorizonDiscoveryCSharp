using Xunit.Abstractions;

namespace LogProcessor.Tests;

public class FormatInfoUnitTests : TestWithStandardOutput
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

    private static readonly string contentW3CBadFields = """
#Software: Microsoft HTTP Server API 2.0
#Version: 1.0   // the log file version as it's described by "https://www.w3.org/TR/WD-logfile".
#Date: 2002-05-02 17:42:15  // when the first log file entry was recorded, which is when the entire log file was created.
--BAD-FIELDS_DEFINITION--: date time c-ip cs-username s-ip s-port cs-method cs-uri-stem cs-uri-query sc-status cs(User-Agent)
2002-05-02 17:42:15 172.22.255.255 - 172.30.255.255 80 GET /images/picture.jpg - 200 Mozilla/4.0+(compatible;MSIE+5.5;+Windows+2000+Server)
""";    
    
    private static readonly string contentW3CBadFieldsEmpty = """
#Software: Microsoft HTTP Server API 2.0
#Version: 1.0   // the log file version as it's described by "https://www.w3.org/TR/WD-logfile".
#Date: 2002-05-02 17:42:15  // when the first log file entry was recorded, which is when the entire log file was created.
#Fields:
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

    public FormatInfoUnitTests(ITestOutputHelper output) : base(output)
    {
    }

    [Fact]
    public void NCSAParsing()
    {
        var expectedNumberOfEntries = 3;
        var expectedNumberOfValues = 9;
        var expectedTimeValue = "02/May/2002:17:42:15 +0100";

        var formatInfo = FormatProvider.GetNCSAFormatInfo();

        var queryForEntries =
            from entry in contentNCSA.Split(Environment.NewLine)
            where formatInfo.IsValidEntry(entry)
            select formatInfo.Parser(entry);

        Assert.Equal(expectedNumberOfEntries, queryForEntries.Count());
        Assert.Equal(expectedNumberOfValues, queryForEntries.First().Count);
        Assert.Equal(expectedTimeValue, queryForEntries.First()[3]);
    }

    [Fact]
    public void W3CColumnNamesParsingForBadFieldSpecifier()
    {
        // arrange
        var tempFileName = CreateW3CLogFile(contentW3CBadFields);

        // act
        void getFormatInfo() => FormatProvider.GetW3CFormatInfo(tempFileName);

        //assert
        ArgumentException exception = Assert.Throws<ArgumentException>(getFormatInfo);
        Assert.Equal($"[FormatProvider::GetW3CFormatInfo]: #Fields: specifier is missing or malformed in [{tempFileName}]!", exception.Message);

        // clean
        File.Delete(tempFileName);
    }

    [Fact]
    public void W3CColumnNamesParsingForBadFieldEmpty()
    {
        // arrange
        var tempFileName = CreateW3CLogFile(contentW3CBadFieldsEmpty);

        // act
        void getFormatInfo() => FormatProvider.GetW3CFormatInfo(tempFileName);

        //assert
        ArgumentException exception = Assert.Throws<ArgumentException>(getFormatInfo);
        Assert.Equal($"[FormatProvider::GetW3CFormatInfo]: #Fields: specifier defines no fields in [{tempFileName}]!", exception.Message); 

        // clean
        File.Delete(tempFileName);
    }

    [Fact]
    public void W3CColumnNamesParsing()
    {
        var tempFileName = CreateW3CLogFile(contentW3C);


        var expectedFields = FormatProvider
            .GetW3CFormatInfo(tempFileName).Fields;

        Assert.Equal(
            new List<string>
            {
                "date", "time", "c-ip", "cs-username", "s-ip",
                "s-port", "cs-method", "cs-uri-stem", "cs-uri-query",
                "sc-status", "cs(User-Agent)"
            },
            expectedFields);


        File.Delete(tempFileName);
    }

    [Fact]
    public void W3CSelectingEntries()
    {
        var tempFileName = CreateW3CLogFile(contentW3C);

        var expectedNumberOfEntries = 3;

        var entryPredicate = FormatProvider
            .GetW3CFormatInfo(tempFileName).IsValidEntry;
        var queryForEntries =
            from entry in contentW3C.Split(Environment.NewLine)
            where entryPredicate(entry)
            select entry;

        Assert.Equal(expectedNumberOfEntries, queryForEntries.Count());


        File.Delete(tempFileName);
    }

    [Fact]
    public void W3CParsing()
    {
        var tempFileName = CreateW3CLogFile(contentW3C);

        var expectedNumberOfEntries = 3;
        var expectedNumberOfValues = 11;
        var expectedTimeValue = "17:42:15";


        var formatInfo = FormatProvider
            .GetW3CFormatInfo(tempFileName);
        var queryForEntries =
            from entry in contentW3C.Split(Environment.NewLine)
            where formatInfo.IsValidEntry(entry)
            select formatInfo.Parser(entry);

        Assert.Equal(expectedNumberOfEntries, queryForEntries.Count());
        Assert.Equal(expectedNumberOfValues, queryForEntries.First().Count);
        Assert.Equal(expectedTimeValue, queryForEntries.First()[1]);


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