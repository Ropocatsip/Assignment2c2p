namespace Assignment2.APP.Models;

public class OrderTransaction
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; }
    public decimal Amount { get; set; }
    public decimal Fees1 { get; set; }
    public decimal Fees2 { get; set; }
    public decimal NetTotal { get; set; }
    public string Status { get; set; } = string.Empty;
}
