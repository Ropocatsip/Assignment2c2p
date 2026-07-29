using System.Text;

namespace Assignment2.APP.Services;

public class CsvWriterService
{
    public static string FormatCsvLine(IEnumerable<string> fields)
    {
        var sb = new StringBuilder();
        bool first = true;

        foreach (var field in fields)
        {
            if (!first)
            {
                sb.Append(',');
            }
            first = false;

            string value = field ?? string.Empty;
            if (value.Contains(',') || value.Contains('"') || value.Contains('\n') || value.Contains('\r'))
            {
                sb.Append('"');
                sb.Append(value.Replace("\"", "\"\""));
                sb.Append('"');
            }
            else
            {
                sb.Append(value);
            }
        }

        return sb.ToString();
    }

    public static string FormatDecimal(decimal value)
    {
        return value.ToString("0.00");
    }
}
