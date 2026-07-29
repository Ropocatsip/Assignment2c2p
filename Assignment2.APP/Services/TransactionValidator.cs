using System.Globalization;
using Assignment2.APP.Models;

namespace Assignment2.APP.Services;

public class TransactionValidator
{
    private static readonly string[] DateFormats = new[]
    {
        "yyyy-MM-dd",
        "dd-MM-yyyy",
        "yyyy/MM/dd",
        "dd/MM/yyyy",
        "yyyy-MM-dd HH:mm:ss",
        "dd-MM-yyyy HH:mm:ss"
    };

    public static ValidationResult ValidateOrderTransaction(OrderTransaction order)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(order.OrderNumber))
        {
            errors.Add("Order Number is required and cannot be empty.");
        }

        if (order.Amount < 0)
        {
            errors.Add($"Amount cannot be negative. Value: {order.Amount}");
        }

        if (string.IsNullOrWhiteSpace(order.Status))
        {
            errors.Add("Status is required and cannot be empty.");
        }

        return errors.Count == 0 ? ValidationResult.Success() : ValidationResult.Fail(errors);
    }

    public static ValidationResult ValidateInvoiceTransaction(InvoiceTransaction invoice)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(invoice.InvoiceNumber))
        {
            errors.Add("Invoice Number is required and cannot be empty.");
        }

        if (invoice.Amount < 0)
        {
            errors.Add($"Amount cannot be negative. Value: {invoice.Amount}");
        }

        if (string.IsNullOrWhiteSpace(invoice.Status))
        {
            errors.Add("Status is required and cannot be empty.");
        }

        return errors.Count == 0 ? ValidationResult.Success() : ValidationResult.Fail(errors);
    }

    public static bool TryParseDate(string dateStr, out DateTime date)
    {
        if (DateTime.TryParseExact(dateStr, DateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
        {
            return true;
        }

        return DateTime.TryParse(dateStr, CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
    }

    public static bool TryParseDecimal(string numStr, out decimal result)
    {
        // Strip commas in formatted numbers like "3,900.00"
        string cleaned = numStr.Replace(",", "").Trim();
        return decimal.TryParse(cleaned, NumberStyles.Any, CultureInfo.InvariantCulture, out result);
    }
}
