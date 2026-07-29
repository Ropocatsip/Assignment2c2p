using Assignment2.APP.Services;

namespace Assignment2.TEST;

public class ReconciliationEngineTests
{
    [Fact]
    public void Process_MockData_GeneratesExpectedCounts()
    {
        string tempDir = Path.Combine(Path.GetTempPath(), "ReconciliationTest_" + Guid.NewGuid());
        Directory.CreateDirectory(tempDir);

        try
        {
            string listAPath = Path.Combine(tempDir, "ListA.csv");
            string listBPath = Path.Combine(tempDir, "ListB.csv");
            string outputDir = Path.Combine(tempDir, "output");

            // Create List A with 3 records (REF001, REF002, REF003)
            File.WriteAllLines(listAPath, new[]
            {
                "#,Order Number,Transaction Date,Amount,Fees1,Fees2,Net Total,Status",
                "1,REF001,2025-04-16,100.00,0,0,100.00,Success",
                "2,REF002,2025-04-16,200.00,0,0,200.00,Success",
                "3,REF003,2025-04-16,300.00,0,0,300.00,Success"
            });

            // Create List B with 3 records (REF002, REF003, REF004)
            File.WriteAllLines(listBPath, new[]
            {
                "#,Invoice Number,Transaction Date,Amount,Fees1,Fees2,Net Total,Card Number,Status",
                "1,REF002,16-04-2025,200.00,0,0,200.00,5555****1111,Success",
                "2,REF003,16-04-2025,300.00,0,0,300.00,5555****1111,Success",
                "3,REF004,16-04-2025,400.00,0,0,400.00,5555****1111,Success"
            });

            var engine = new ReconciliationEngine();
            var summary = engine.Process(listAPath, listBPath, outputDir);

            // Assert Metrics:
            // REF002 & REF003 match -> Matched = 2
            // REF001 in A not B -> MissingInB = 1
            // REF004 in B not A -> MissingInA = 1
            Assert.Equal(3, summary.TotalRecordsListA);
            Assert.Equal(3, summary.TotalRecordsListB);
            Assert.Equal(2, summary.MatchedCount);
            Assert.Equal(1, summary.MissingInBCount);
            Assert.Equal(1, summary.MissingInACount);

            // Assert Files Exist
            Assert.True(File.Exists(Path.Combine(outputDir, "Matched_Records.csv")));
            Assert.True(File.Exists(Path.Combine(outputDir, "Missing_In_B.csv")));
            Assert.True(File.Exists(Path.Combine(outputDir, "Missing_In_A.csv")));
        }
        finally
        {
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }
    }
}
