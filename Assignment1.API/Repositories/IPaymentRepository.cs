using Assignment1.Models;

namespace Assignment1.Repositories;

public interface IPaymentRepository
{
    Task SaveTransactionAsync(PaymentRequest request, PaymentResponse response, CancellationToken cancellationToken = default);
    Task<bool> HasApprovedTransactionAsync(string orderNumber, CancellationToken cancellationToken = default);
}

