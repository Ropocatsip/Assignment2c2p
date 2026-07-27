using Assignment1.Models;
using MongoDB.Driver;

namespace Assignment1.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly IMongoCollection<PaymentTransaction> _transactionCollection;

    public PaymentRepository(IMongoDatabase database)
    {
        _transactionCollection = database.GetCollection<PaymentTransaction>("transaction");
    }

    public void SaveTransaction(PaymentRequest request, PaymentResponse response)
    {
        var transaction = new PaymentTransaction
        {
            Request = request,
            Response = response,
            CreatedAt = DateTime.UtcNow
        };

        _transactionCollection.InsertOne(transaction);
    }
}
