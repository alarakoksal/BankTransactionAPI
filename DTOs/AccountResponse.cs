namespace BankTransactionTracker.DTOs
{
    public class AccountResponse
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public decimal Balance { get; set; }
    }
}