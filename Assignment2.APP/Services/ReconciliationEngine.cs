using System.Diagnostics;
using Assignment2.APP.Models;

namespace Assignment2.APP.Services;

public class ReconciliationEngine
{
    public ReconciliationSummary Process(string listAPath, string listBPath, string outputDirectory)
    {
        var summary = new ReconciliationSummary();
        var stopwatch = Stopwatch.StartNew();

        if (!File.Exists(listAPath))
        {
            throw new FileNotFoundException($"Input file List A not found: {listAPath}");
        }

        if (!File.Exists(listBPath))
        {
            throw new FileNotFoundException($"Input file List B not found: {listBPath}");
        }

        Directory.CreateDirectory(outputDirectory);

        string matchedFilePath = Path.Combine(outputDirectory, "Matched_Records.csv");
        string missingInBFilePath = Path.Combine(outputDirectory, "Missing_In_B.csv");
        string missingInAFilePath = Path.Combine(outputDirectory, "Missing_In_A.csv");

        // Step 1: Read List B into a key-indexed memory map
        // Dictionary key: Invoice Number (trimmed, case-insensitive)
        var listBMap = new Dictionary<string, (Models.InvoiceTransaction Invoice, string RawLine)>(StringComparer.OrdinalIgnoreCase);
        var matchedBKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        using (var readerB = new StreamReader(listBPath))
        {
            string? headerB = readerB.ReadLine();
            int rowNum = 0;

            while (!readerB.EndOfStream)
            {
                string? line = readerB.ReadLine();
                if (string.IsNullOrWhiteSpace(line)) continue;
                rowNum++;

                var fields = CsvReaderService.ParseCsvLine(line);
                if (fields.Count < 9)
                {
                    summary.InvalidRecordsListB++;
                    continue;
                }

                if (!TransactionValidator.TryParseDecimal(fields[3], out decimal amount) ||
                    !TransactionValidator.TryParseDecimal(fields[4], out decimal fees1) ||
                    !TransactionValidator.TryParseDecimal(fields[5], out decimal fees2) ||
                    !TransactionValidator.TryParseDecimal(fields[6], out decimal netTotal) ||
                    !TransactionValidator.TryParseDate(fields[2], out DateTime txDate))
                {
                    summary.InvalidRecordsListB++;
                    continue;
                }

                int.TryParse(fields[0], out int id);
                var invoice = new Models.InvoiceTransaction
                {
                    Id = id,
                    InvoiceNumber = fields[1],
                    TransactionDate = txDate,
                    Amount = amount,
                    Fees1 = fees1,
                    Fees2 = fees2,
                    NetTotal = netTotal,
                    CardNumber = fields[7],
                    Status = fields[8]
                };

                var valResult = TransactionValidator.ValidateInvoiceTransaction(invoice);
                if (!valResult.IsValid)
                {
                    summary.InvalidRecordsListB++;
                    continue;
                }

                summary.TotalRecordsListB++;
                listBMap[invoice.InvoiceNumber] = (invoice, line);
            }
        }

        // Step 2: Stream List A, match against List B map, write Matched and Missing_In_B
        using (var readerA = new StreamReader(listAPath))
        using (var writerMatched = new StreamWriter(matchedFilePath, false))
        using (var writerMissingB = new StreamWriter(missingInBFilePath, false))
        {
            string? headerA = readerA.ReadLine();

            writerMatched.WriteLine("Order Number,Invoice Number,Transaction Date A,Transaction Date B,Amount A,Amount B,Fees1 A,Fees1 B,Fees2 A,Fees2 B,Net Total A,Net Total B,Card Number,Status A,Status B");
            writerMissingB.WriteLine(headerA ?? "#,Order Number,Transaction Date,Amount,Fees1,Fees2,Net Total,Status");

            int rowNum = 0;

            while (!readerA.EndOfStream)
            {
                string? line = readerA.ReadLine();
                if (string.IsNullOrWhiteSpace(line)) continue;
                rowNum++;

                var fields = CsvReaderService.ParseCsvLine(line);
                if (fields.Count < 8)
                {
                    summary.InvalidRecordsListA++;
                    continue;
                }

                if (!TransactionValidator.TryParseDecimal(fields[3], out decimal amount) ||
                    !TransactionValidator.TryParseDecimal(fields[4], out decimal fees1) ||
                    !TransactionValidator.TryParseDecimal(fields[5], out decimal fees2) ||
                    !TransactionValidator.TryParseDecimal(fields[6], out decimal netTotal) ||
                    !TransactionValidator.TryParseDate(fields[2], out DateTime txDate))
                {
                    summary.InvalidRecordsListA++;
                    continue;
                }

                int.TryParse(fields[0], out int id);
                var order = new Models.OrderTransaction
                {
                    Id = id,
                    OrderNumber = fields[1],
                    TransactionDate = txDate,
                    Amount = amount,
                    Fees1 = fees1,
                    Fees2 = fees2,
                    NetTotal = netTotal,
                    Status = fields[7]
                };

                var valResult = TransactionValidator.ValidateOrderTransaction(order);
                if (!valResult.IsValid)
                {
                    summary.InvalidRecordsListA++;
                    continue;
                }

                summary.TotalRecordsListA++;

                if (listBMap.TryGetValue(order.OrderNumber, out var tuple))
                {
                    // MATCHED
                    matchedBKeys.Add(order.OrderNumber);
                    summary.MatchedCount++;

                    var invoice = tuple.Invoice;
                    var matchedFields = new[]
                    {
                        order.OrderNumber,
                        invoice.InvoiceNumber,
                        order.TransactionDate.ToString("yyyy-MM-dd"),
                        invoice.TransactionDate.ToString("yyyy-MM-dd"),
                        CsvWriterService.FormatDecimal(order.Amount),
                        CsvWriterService.FormatDecimal(invoice.Amount),
                        CsvWriterService.FormatDecimal(order.Fees1),
                        CsvWriterService.FormatDecimal(invoice.Fees1),
                        CsvWriterService.FormatDecimal(order.Fees2),
                        CsvWriterService.FormatDecimal(invoice.Fees2),
                        CsvWriterService.FormatDecimal(order.NetTotal),
                        CsvWriterService.FormatDecimal(invoice.NetTotal),
                        invoice.CardNumber,
                        order.Status,
                        invoice.Status
                    };

                    writerMatched.WriteLine(CsvWriterService.FormatCsvLine(matchedFields));
                }
                else
                {
                    // MISSING IN B
                    summary.MissingInBCount++;
                    writerMissingB.WriteLine(line);
                }
            }
        }

        using (var writerMissingA = new StreamWriter(missingInAFilePath, false))
        {
            writerMissingA.WriteLine("#,Invoice Number,Transaction Date,Amount,Fees1,Fees2,Net Total,Card Number,Status");

            foreach (var kvp in listBMap)
            {
                if (!matchedBKeys.Contains(kvp.Key))
                {
                    summary.MissingInACount++;
                    writerMissingA.WriteLine(kvp.Value.RawLine);
                }
            }
        }

        stopwatch.Stop();
        summary.ElapsedTime = stopwatch.Elapsed;
        return summary;
    }
}
