using Assignment1.Models;

namespace Assignment1.Services;

public interface IPaymentService
{
    PaymentResponse Pay(PaymentRequest request);
}
