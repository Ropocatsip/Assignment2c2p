using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Assignment1.Models;

public class PaymentTransaction
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("request")]
    public PaymentRequest Request { get; set; } = null!;

    [BsonElement("response")]
    public PaymentResponse Response { get; set; } = null!;

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
