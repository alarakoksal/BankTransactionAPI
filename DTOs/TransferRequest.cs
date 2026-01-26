using System.ComponentModel.DataAnnotations;

namespace BankTransactionTracker.DTOs
{
    public class TransferRequest
    {
        [Required]
        public int SenderAccountId { get; set; }

        [Required]
        public int ReceiverAccountId { get; set; }

        [Range(0.01, 1000000000)]
        public decimal Amount { get; set; }
    }
}