using Assignment2.APP.Services;

namespace Assignment2.TEST;

public class CsvServiceTests
{
    [Fact]
    public void ParseCsvLine_SimpleLine_ReturnsFields()
    {
        string line = "1,2696111,2025-04-16,520.00,Success";
        var fields = CsvReaderService.ParseCsvLine(line);

        Assert.Equal(5, fields.Count);
        Assert.Equal("1", fields[0]);
        Assert.Equal("2696111", fields[1]);
        Assert.Equal("2025-04-16", fields[2]);
        Assert.Equal("520.00", fields[3]);
        Assert.Equal("Success", fields[4]);
    }

    [Fact]
    public void ParseCsvLine_QuotedFieldsWithCommas_ParsesCorrectly()
    {
        string line = @"1,2696111,2025-04-16,""3,900.00"",-0.8,-21.3,""3,877.90"",Success";
        var fields = CsvReaderService.ParseCsvLine(line);

        Assert.Equal(8, fields.Count);
        Assert.Equal("3,900.00", fields[3]);
        Assert.Equal("3,877.90", fields[6]);
    }

    [Fact]
    public void FormatCsvLine_FieldWithComma_EscapesWithQuotes()
    {
        var fields = new[] { "1", "2696111", "3,900.00", "Success" };
        string csvLine = CsvWriterService.FormatCsvLine(fields);

        Assert.Equal(@"1,2696111,""3,900.00"",Success", csvLine);
    }
}
