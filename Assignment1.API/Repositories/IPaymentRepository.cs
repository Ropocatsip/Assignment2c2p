using Assignment1.Models;

namespace Assignment1.Repositories;

public interface IPaymentRepository
{
    void SaveTransaction(PaymentRequest request, PaymentResponse response);
    bool HasApprovedTransaction(string orderNumber);
}
