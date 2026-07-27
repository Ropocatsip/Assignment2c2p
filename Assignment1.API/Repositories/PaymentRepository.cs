using System.Collections.Concurrent;
using Assignment1.Models;

namespace Assignment1.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private static readonly ConcurrentBag<(PaymentRequest Request, PaymentResponse Response)> _transactions = new();

    public void SaveTransaction(PaymentRequest request, PaymentResponse response)
    {
        _transactions.Add((request, response));
    }
}
