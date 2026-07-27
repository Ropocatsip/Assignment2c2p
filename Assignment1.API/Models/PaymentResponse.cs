using System.Text.Json.Serialization;

namespace Assignment1.Models;

public class PaymentResponse
{
    [JsonPropertyName("transaction_id")]
    public string TransactionId { get; set; } = string.Empty;

    [JsonPropertyName("acquirer_reference")]
    public string AcquirerReference { get; set; } = string.Empty;

    [JsonPropertyName("response_code")]
    public string ResponseCode { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }

    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }
}
