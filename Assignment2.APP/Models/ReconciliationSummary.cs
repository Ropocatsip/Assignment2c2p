namespace Assignment2.APP.Models;

public class ReconciliationSummary
{
    public int TotalRecordsListA { get; set; }
    public int TotalRecordsListB { get; set; }
    public int MatchedCount { get; set; }
    public int MissingInBCount { get; set; }
    public int MissingInACount { get; set; }
    public int InvalidRecordsListA { get; set; }
    public int InvalidRecordsListB { get; set; }
    public TimeSpan ElapsedTime { get; set; }

    public void PrintSummary()
    {
        Console.WriteLine("==================================================");
        Console.WriteLine("              RECONCILIATION SUMMARY              ");
        Console.WriteLine("==================================================");
        Console.WriteLine($"Total Records in List A (Orders)    : {TotalRecordsListA:N0}");
        Console.WriteLine($"Total Records in List B (Invoices)  : {TotalRecordsListB:N0}");
        Console.WriteLine($"--------------------------------------------------");
        Console.WriteLine($"Matched Records                     : {MatchedCount:N0}");
        Console.WriteLine($"Missing in List B (Only in List A)  : {MissingInBCount:N0}");
        Console.WriteLine($"Missing in List A (Only in List B)  : {MissingInACount:N0}");
        Console.WriteLine($"--------------------------------------------------");
        Console.WriteLine($"Invalid Records in List A           : {InvalidRecordsListA:N0}");
        Console.WriteLine($"Invalid Records in List B           : {InvalidRecordsListB:N0}");
        Console.WriteLine($"Processing Time                     : {ElapsedTime.TotalMilliseconds:F2} ms");
        Console.WriteLine("==================================================");
    }
}
