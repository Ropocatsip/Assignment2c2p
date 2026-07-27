using Assignment1.Models;
using MongoDB.Driver;

namespace Assignment1.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly IMongoCollection<PaymentTransaction> _transactionCollection;
    private readonly ILogger<PaymentRepository> _logger;

    public PaymentRepository(IMongoDatabase database, ILogger<PaymentRepository> logger)
    {
        _transactionCollection = database.GetCollection<PaymentTransaction>("transaction");
        _logger = logger;
    }

    public void SaveTransaction(PaymentRequest request, PaymentResponse response)
    {
        try
        {
            var transaction = new PaymentTransaction
            {
                Request = request,
                Response = response,
                CreatedAt = DateTime.UtcNow
            };

            _transactionCollection.InsertOne(transaction);
            _logger.LogInformation("Successfully saved transaction {TransactionId} to MongoDB.", response.TransactionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while inserting transaction {TransactionId} into MongoDB.", response.TransactionId);
            throw new InvalidOperationException($"Failed to save transaction to database: {ex.Message}", ex);
        }
    }

    public bool HasApprovedTransaction(string orderNumber)
    {
        try
        {
            return _transactionCollection
                .Find(t => t.Request.OrderNumber == orderNumber && t.Response.Status == "APPROVED")
                .Any();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking existing approved transaction for order {OrderNumber}", orderNumber);
            throw new InvalidOperationException($"Failed to query database for order status: {ex.Message}", ex);
        }
    }
}
