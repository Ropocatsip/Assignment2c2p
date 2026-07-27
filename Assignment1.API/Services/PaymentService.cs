using System.Security.Cryptography;
using Assignment1.Models;
using Assignment1.Repositories;

namespace Assignment1.Services;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;

    public PaymentService(IPaymentRepository paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }

    public PaymentResponse Pay(PaymentRequest request)
    {
        // Unique UUID for transaction_id
        string transactionId = Guid.NewGuid().ToString();

        // Unique mock reference number from the "bank"
        string acquirerRef = $"REF-{RandomNumberGenerator.GetHexString(6).ToUpper()}";

        // Logic based on the Amount's decimal place:
        // .00 = success ("APPROVED", response_code = "00")
        // Others = rejected ("DECLINED", response_code = cents string)
        decimal decimalPart = Math.Abs(request.Amount - Math.Truncate(request.Amount));
        int cents = (int)Math.Round(decimalPart * 100);
        string responseCode = cents.ToString("D2");

        string status = responseCode == "00" ? "APPROVED" : "DECLINED";

        var response = new PaymentResponse
        {
            TransactionId = transactionId,
            AcquirerReference = acquirerRef,
            ResponseCode = responseCode,
            Status = status,
            Timestamp = DateTime.UtcNow,
            Amount = request.Amount
        };

        _paymentRepository.SaveTransaction(request, response);

        return response;
    }
}
