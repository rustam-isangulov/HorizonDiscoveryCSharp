using System.CommandLine;

namespace LogProcessor;

public class Program
{
    internal enum SupportedFormats { W3C, NCSA };

    private const string _description = "Log processor parses log files, filters, aggregates and outputs ranked list of entries.";

    public static int Main(string[] args)
    {
        var filesOption = new Option<FileInfo[]>
            (name: "--files", description: "Files to process",
            parseArgument: result =>
            {
                var files = new List<FileInfo>();

                foreach (var arg in result.Tokens)
                {
                    Console.WriteLine(arg);
                    if (!File.Exists(arg.Value))
                    {
                        result.ErrorMessage = $"File \"{arg.Value}\" does not exist!";
                        return files.ToArray();
                    }

                    files.Add(new FileInfo(arg.Value));
                }

                return files.ToArray();
            })
        { IsRequired = true, AllowMultipleArgumentsPerToken = true };

        var typeOption = new Option<SupportedFormats>
            (name: "--type", description: "Processor type")
        { IsRequired = true };

        var rootCommand = new RootCommand(_description)
        {
            filesOption,
            typeOption
        };

        rootCommand.SetHandler(RunProcessor,
                       filesOption,
                       typeOption);

        return rootCommand.InvokeAsync(args).Result;
    }

    internal static void RunProcessor(IList<FileInfo> files, SupportedFormats type)
    {
        Console.WriteLine("files to process:");
        foreach (var file in files)
        {
            Console.WriteLine($"\t: {file}");
        }
        Console.WriteLine($"log type: {type}");
    }
}