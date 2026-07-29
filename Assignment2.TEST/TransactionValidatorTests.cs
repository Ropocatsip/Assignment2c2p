using Assignment2.APP.Models;
using Assignment2.APP.Services;

namespace Assignment2.TEST;

public class TransactionValidatorTests
{
    [Fact]
    public void ValidateOrderTransaction_ValidOrder_ReturnsSuccess()
    {
        var order = new OrderTransaction
        {
            Id = 1,
            OrderNumber = "ORD123",
            TransactionDate = DateTime.Now,
            Amount = 100.00m,
            Status = "Success"
        };

        var result = TransactionValidator.ValidateOrderTransaction(order);
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void ValidateOrderTransaction_EmptyOrderNumber_ReturnsFailed()
    {
        var order = new OrderTransaction
        {
            OrderNumber = "",
            Amount = 100.00m,
            Status = "Success"
        };

        var result = TransactionValidator.ValidateOrderTransaction(order);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("Order Number"));
    }

    [Fact]
    public void ValidateOrderTransaction_NegativeAmount_ReturnsFailed()
    {
        var order = new OrderTransaction
        {
            OrderNumber = "ORD123",
            Amount = -10.00m,
            Status = "Success"
        };

        var result = TransactionValidator.ValidateOrderTransaction(order);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("negative"));
    }

    [Theory]
    [InlineData("2025-04-16")]
    [InlineData("16-04-2025")]
    public void TryParseDate_ValidFormats_ReturnsTrue(string dateStr)
    {
        bool success = TransactionValidator.TryParseDate(dateStr, out DateTime date);
        Assert.True(success);
        Assert.Equal(2025, date.Year);
        Assert.Equal(4, date.Month);
        Assert.Equal(16, date.Day);
    }

    [Fact]
    public void TryParseDecimal_FormattedAmount_ReturnsDecimal()
    {
        bool success = TransactionValidator.TryParseDecimal("3,900.50", out decimal value);
        Assert.True(success);
        Assert.Equal(3900.50m, value);
    }
}
