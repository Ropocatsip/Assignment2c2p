using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Assignment1.Models;

public class PaymentRequest
{
    /// <example>ORD-20260727-001</example>
    [Required]
    [JsonPropertyName("order_number")]
    public string OrderNumber { get; set; } = string.Empty;

    /// <example>4111111111119999</example>
    [Required]
    [RegularExpression(@"^\d{16}$", ErrorMessage = "Card number must be 16 digits.")]
    [JsonPropertyName("card_number")]
    public string CardNumber { get; set; } = string.Empty;

    /// <example>12/28</example>
    [Required]
    [RegularExpression(@"^(0[1-9]|1[0-2])\/\d{2}$", ErrorMessage = "Expiry date must be in MM/YY format.")]
    [FutureExpiryDate(ErrorMessage = "Expiry date must be in the future.")]
    [JsonPropertyName("expiry_date")]
    public string ExpiryDate { get; set; } = string.Empty;

    /// <example>123</example>
    [Required]
    [RegularExpression(@"^\d{3,4}$", ErrorMessage = "CVV must be 3 or 4 digits.")]
    [JsonPropertyName("cvv")]
    public string Cvv { get; set; } = string.Empty;

    /// <example>THB</example>
    [Required]
    [RegularExpression(@"^[A-Z]{3}$", ErrorMessage = "Currency must be a 3-letter ISO 4217 code.")]
    [JsonPropertyName("currency")]
    public string Currency { get; set; } = string.Empty;

    /// <example>Nudthaya K.</example>
    [Required]
    [JsonPropertyName("cardholder_name")]
    public string CardholderName { get; set; } = string.Empty;

    /// <example>test@example.com</example>
    [Required]
    [EmailAddress]
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    /// <example>100.00</example>
    [Required]
    [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
    [TwoDecimalPlaces(ErrorMessage = "Amount must have at most 2 decimal places.")]
    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }
}

public class FutureExpiryDateAttribute : ValidationAttribute
{
    public FutureExpiryDateAttribute()
    {
        ErrorMessage = "Expiry date must be in the future.";
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string expiryString || string.IsNullOrWhiteSpace(expiryString))
        {
            return ValidationResult.Success;
        }

        var parts = expiryString.Split('/');
        if (parts.Length != 2 || !int.TryParse(parts[0], out int month) || !int.TryParse(parts[1], out int yearTwoDigits))
        {
            return ValidationResult.Success;
        }

        if (month < 1 || month > 12)
        {
            return ValidationResult.Success;
        }

        int fullYear = 2000 + yearTwoDigits;
        var lastDayOfMonth = new DateTime(fullYear, month, DateTime.DaysInMonth(fullYear, month), 23, 59, 59, DateTimeKind.Utc);

        if (lastDayOfMonth < DateTime.UtcNow)
        {
            return new ValidationResult(ErrorMessage);
        }

        return ValidationResult.Success;
    }
}

public class TwoDecimalPlacesAttribute : ValidationAttribute
{
    public TwoDecimalPlacesAttribute()
    {
        ErrorMessage = "Amount must have at most 2 decimal places.";
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
        {
            return ValidationResult.Success;
        }

        if (value is decimal amount)
        {
            if (amount != Math.Round(amount, 2))
            {
                return new ValidationResult(ErrorMessage);
            }
        }
        return ValidationResult.Success;
    }
}
