using System.CommandLine;

namespace LogProcessor;

public class Program
{
    private const string _description = "Log processor parses log files, filters, aggregates and outputs ranked list of entries.";

    public static int Main (string[] args)
    {
	var filesOption = new Option<string[]>
	    (
		name: "--files",
		description: "Files to process"
	    )
	    {
		IsRequired = true,
		AllowMultipleArgumentsPerToken = true
	    };

	var typeOption = new Option<string>
	    (
		name: "--type",
		description: "Processor type"
	    )
	    { IsRequired = true }
	    .FromAmong("W3C","NCSA");

	var rootCommand = new RootCommand(_description);
	rootCommand.Add(filesOption);
	rootCommand.Add(typeOption);

	rootCommand.SetHandler((filesOptionValues, typeOptionValue) =>
			       RunProcessor(filesOptionValues, typeOptionValue),
			       filesOption,
			       typeOption);

        return rootCommand.InvokeAsync(args).Result;
    }

    internal static void RunProcessor(IList<string> files, string type)
    {
	Console.WriteLine("files to process:");
	foreach (var file in files)
	{
	    Console.WriteLine($"\t: {file}");
	}
	Console.WriteLine($"log type: {type}");
    }
}
