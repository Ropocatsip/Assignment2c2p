using Assignment1.Models;
using Assignment1.Repositories;
using Assignment1.Services;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace Assignment1.TEST;

public class PaymentServiceTests
{
    private readonly Mock<IPaymentRepository> _mockPaymentRepository;
    private readonly Mock<IEncryptionService> _mockEncryptionService;
    private readonly PaymentService _paymentService;

    public PaymentServiceTests()
    {
        _mockPaymentRepository = new Mock<IPaymentRepository>();
        _mockEncryptionService = new Mock<IEncryptionService>();

        // Default mock setup for encryption
        _mockEncryptionService
            .Setup(e => e.Encrypt(It.IsAny<string>()))
            .Returns<string>(input => $"ENC_{input}");

        _paymentService = new PaymentService(_mockPaymentRepository.Object, _mockEncryptionService.Object);
    }

    private static PaymentRequest CreateValidPaymentRequest(string orderNumber = "ORD-2026-001", decimal amount = 100.00m)
    {
        return new PaymentRequest
        {
            OrderNumber = orderNumber,
            CardNumber = "4111111111119999",
            ExpiryDate = "12/28",
            Cvv = "123",
            Currency = "THB",
            CardholderName = "Test User",
            Email = "test@example.com",
            Amount = amount
        };
    }

    [Fact]
    public void Pay_WhenOrderAlreadyApproved_ThrowsBadHttpRequestException()
    {
        var request = CreateValidPaymentRequest("ORD-DUPLICATE", 100.00m);
        _mockPaymentRepository
            .Setup(r => r.HasApprovedTransaction("ORD-DUPLICATE"))
            .Returns(true);

        var exception = Assert.Throws<BadHttpRequestException>(() => _paymentService.Pay(request));
        Assert.Contains("ORD-DUPLICATE", exception.Message);
        Assert.Contains("already been paid and approved", exception.Message);

        _mockPaymentRepository.Verify(
            r => r.SaveTransaction(It.IsAny<PaymentRequest>(), It.IsAny<PaymentResponse>()),
            Times.Never
        );
    }

    [Fact]
    public void Pay_WhenAmountHasZeroCents_ReturnsApprovedResponseAndSavesTransaction()
    {
        var request = CreateValidPaymentRequest("ORD-APPROVED", 100.00m);
        _mockPaymentRepository
            .Setup(r => r.HasApprovedTransaction("ORD-APPROVED"))
            .Returns(false);

        var response = _paymentService.Pay(request);

        Assert.NotNull(response);
        Assert.Equal("APPROVED", response.Status);
        Assert.Equal("00", response.ResponseCode);
        Assert.Equal(100.00m, response.Amount);
        Assert.False(string.IsNullOrWhiteSpace(response.TransactionId));
        Assert.StartsWith("REF-", response.AcquirerReference);

        _mockPaymentRepository.Verify(
            r => r.SaveTransaction(
                It.Is<PaymentRequest>(req => req.CardNumber == "ENC_4111111111119999" && req.Cvv == "ENC_123"),
                It.Is<PaymentResponse>(res => res.Status == "APPROVED" && res.ResponseCode == "00")
            ),
            Times.Once
        );
    }

    [Fact]
    public void Pay_WhenAmountHasNonZeroCents_ReturnsDeclinedResponseWithCentsCode()
    {
        var request = CreateValidPaymentRequest("ORD-DECLINED-50", 100.50m);
        _mockPaymentRepository
            .Setup(r => r.HasApprovedTransaction("ORD-DECLINED-50"))
            .Returns(false);

        var response = _paymentService.Pay(request);
        Assert.NotNull(response);
        Assert.Equal("DECLINED", response.Status);
        Assert.Equal("50", response.ResponseCode);
        Assert.Equal(100.50m, response.Amount);

        _mockPaymentRepository.Verify(
            r => r.SaveTransaction(
                It.IsAny<PaymentRequest>(),
                It.Is<PaymentResponse>(res => res.Status == "DECLINED" && res.ResponseCode == "50")
            ),
            Times.Once
        );
    }

    [Fact]
    public void Pay_WhenAmountHas99Cents_ReturnsDeclinedResponseWithCode99()
    {
        var request = CreateValidPaymentRequest("ORD-DECLINED-99", 25.99m);
        _mockPaymentRepository
            .Setup(r => r.HasApprovedTransaction("ORD-DECLINED-99"))
            .Returns(false);

        var response = _paymentService.Pay(request);

        Assert.NotNull(response);
        Assert.Equal("DECLINED", response.Status);
        Assert.Equal("99", response.ResponseCode);
    }

    [Fact]
    public void Pay_EncryptsCardNumberAndCvvBeforeSaving()
    {
        var request = CreateValidPaymentRequest("ORD-ENCRYPT-CHECK", 50.00m);
        _mockPaymentRepository
            .Setup(r => r.HasApprovedTransaction("ORD-ENCRYPT-CHECK"))
            .Returns(false);

        _paymentService.Pay(request);

        _mockEncryptionService.Verify(e => e.Encrypt("4111111111119999"), Times.Once);
        _mockEncryptionService.Verify(e => e.Encrypt("123"), Times.Once);
    }
}
