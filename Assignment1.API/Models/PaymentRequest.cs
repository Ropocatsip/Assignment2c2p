using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Assignment1.Models;

public class PaymentRequest
{
    [Required]
    [JsonPropertyName("order_number")]
    public string OrderNumber { get; set; } = string.Empty;

    [Required]
    [RegularExpression(@"^\d{16}$", ErrorMessage = "Card number must be 16 digits.")]
    [JsonPropertyName("card_number")]
    public string CardNumber { get; set; } = string.Empty;

    [Required]
    [RegularExpression(@"^(0[1-9]|1[0-2])\/\d{2}$", ErrorMessage = "Expiry date must be in MM/YY format.")]
    [JsonPropertyName("expiry_date")]
    public string ExpiryDate { get; set; } = string.Empty;

    [Required]
    [RegularExpression(@"^\d{3,4}$", ErrorMessage = "CVV must be 3 or 4 digits.")]
    [JsonPropertyName("cvv")]
    public string Cvv { get; set; } = string.Empty;

    [Required]
    [RegularExpression(@"^[A-Z]{3}$", ErrorMessage = "Currency must be a 3-letter ISO 4217 code.")]
    [JsonPropertyName("currency")]
    public string Currency { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("cardholder_name")]
    public string CardholderName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }
}
