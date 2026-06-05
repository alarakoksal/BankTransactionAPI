namespace BankTransactionTracker.DTOs
{
    public class TransactionResponse
    {
        public int Id { get; set; }
        public int SenderAccountId { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public string SenderAccountNumber { get; set; } = string.Empty;
        public int ReceiverAccountId { get; set; }
        public string ReceiverName { get; set; } = string.Empty;
        public string ReceiverAccountNumber { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
    }
}
