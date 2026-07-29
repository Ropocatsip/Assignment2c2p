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

    public async Task SaveTransactionAsync(PaymentRequest request, PaymentResponse response, CancellationToken cancellationToken = default)
    {
        try
        {
            var transaction = new PaymentTransaction
            {
                Request = request,
                Response = response,
                CreatedAt = DateTime.UtcNow
            };

            await _transactionCollection.InsertOneAsync(transaction, cancellationToken: cancellationToken);
            _logger.LogInformation("Successfully saved transaction {TransactionId} to MongoDB.", response.TransactionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while inserting transaction {TransactionId} into MongoDB.", response.TransactionId);
            throw new InvalidOperationException($"Failed to save transaction to database: {ex.Message}", ex);
        }
    }

    public async Task<bool> HasApprovedTransactionAsync(string orderNumber, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _transactionCollection
                .Find(t => t.Request.OrderNumber == orderNumber && t.Response.Status == "APPROVED")
                .AnyAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking existing approved transaction for order {OrderNumber}", orderNumber);
            throw new InvalidOperationException($"Failed to query database for order status: {ex.Message}", ex);
        }
    }
}

