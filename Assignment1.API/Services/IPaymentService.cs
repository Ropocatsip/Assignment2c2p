using Assignment1.Models;

namespace Assignment1.Services;

public interface IPaymentService
{
    Task<PaymentResponse> PayAsync(PaymentRequest request, CancellationToken cancellationToken = default);
}

