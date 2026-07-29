using Assignment2.APP.Services;

namespace Assignment2.APP;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("==================================================");
        Console.WriteLine("        Assignment 2: Financial Reconciliation     ");
        Console.WriteLine("==================================================");

        string baseDir = AppDomain.CurrentDomain.BaseDirectory;
        string projectDir = Path.GetFullPath(Path.Combine(baseDir, "..", "..", ".."));

        string listAPath = args.Length > 0 ? args[0] : Path.Combine(projectDir, "Data", "List A - List1.csv");
        string listBPath = args.Length > 1 ? args[1] : Path.Combine(projectDir, "Data", "List B - List2.csv");
        string outputDir = args.Length > 2 ? args[2] : Path.Combine(projectDir, "output");

        Console.WriteLine($"List A File: {listAPath}");
        Console.WriteLine($"List B File: {listBPath}");
        Console.WriteLine($"Output Dir : {outputDir}");
        Console.WriteLine("--------------------------------------------------");

        try
        {
            var engine = new ReconciliationEngine();
            var summary = engine.Process(listAPath, listBPath, outputDir);

            Console.WriteLine();
            summary.PrintSummary();
            Console.WriteLine();
            Console.WriteLine($"Reconciliation completed successfully.");
            Console.WriteLine($"Output files generated in: {outputDir}");
            Console.WriteLine("  1. Matched_Records.csv");
            Console.WriteLine("  2. Missing_In_B.csv");
            Console.WriteLine("  3. Missing_In_A.csv");
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[Error] An exception occurred during reconciliation: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            Console.ResetColor();
            Environment.ExitCode = 1;
        }
    }
}
