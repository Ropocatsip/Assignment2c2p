using System.Text;

namespace Assignment2.APP.Services;

public class CsvReaderService
{
    /// <summary>
    /// Reads a CSV line and splits it into fields according to RFC 4180 (handling quoted values with commas).
    /// </summary>
    public static List<string> ParseCsvLine(string line)
    {
        var fields = new List<string>();
        if (line == null) return fields;

        var sb = new StringBuilder();
        bool inQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '"')
            {
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    sb.Append('"');
                    i++;
                }
                else
                {
                    inQuotes = !inQuotes;
                }
            }
            else if (c == ',' && !inQuotes)
            {
                fields.Add(sb.ToString().Trim());
                sb.Clear();
            }
            else
            {
                sb.Append(c);
            }
        }

        fields.Add(sb.ToString().Trim());
        return fields;
    }
}
