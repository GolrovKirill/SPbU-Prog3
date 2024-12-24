namespace MyNUnit;

/// <summary>
/// Main entry point for program.
/// </summary>
public static class Program
{
    public static async Task Main(string[] args)
    {
        if (args.Length != 1)
        {
            Console.WriteLine("Incorrect input. Enter the path to the directory.");
            return;
        }

        var path = args[0];
        
        if (!Directory.Exists(path))
        {
            Console.WriteLine($"File '{path}' not found.");
            return;
        }

        await Tester.RunTestAndPrintResultsAsync(path);
    }
}