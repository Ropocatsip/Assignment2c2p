using System.Security.Cryptography;
using Assignment1.Models;
using Assignment1.Repositories;

namespace Assignment1.Services;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IEncryptionService _encryptionService;

    public PaymentService(IPaymentRepository paymentRepository, IEncryptionService encryptionService)
    {
        _paymentRepository = paymentRepository;
        _encryptionService = encryptionService;
    }

    public PaymentResponse Pay(PaymentRequest request)
    {
        if (_paymentRepository.HasApprovedTransaction(request.OrderNumber))
        {
            throw new BadHttpRequestException($"Order number '{request.OrderNumber}' has already been paid and approved.");
        }

        string transactionId = Guid.NewGuid().ToString();
        string acquirerRef = $"REF-{RandomNumberGenerator.GetHexString(6).ToUpper()}";
        decimal decimalPart = Math.Abs(request.Amount - Math.Truncate(request.Amount));
        int cents = (int)Math.Round(decimalPart * 100);
        string responseCode = cents.ToString("D2");
        string status = responseCode == "00" ? "APPROVED" : "DECLINED";

        var encryptedRequest = new PaymentRequest
        {
            OrderNumber = request.OrderNumber,
            CardNumber = _encryptionService.Encrypt(request.CardNumber),
            ExpiryDate = request.ExpiryDate,
            Cvv = _encryptionService.Encrypt(request.Cvv),
            Currency = request.Currency,
            CardholderName = request.CardholderName,
            Email = request.Email,
            Amount = request.Amount
        };

        var response = new PaymentResponse
        {
            TransactionId = transactionId,
            AcquirerReference = acquirerRef,
            ResponseCode = responseCode,
            Status = status,
            Timestamp = DateTime.UtcNow,
            Amount = request.Amount
        };

        _paymentRepository.SaveTransaction(encryptedRequest, response);
        return response;
    }
}
