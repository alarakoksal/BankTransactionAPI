using System.ComponentModel.DataAnnotations;

namespace BankTransactionTracker.DTOs
{
    public class CreateAccountRequest
    {
        [Required]
        [MinLength(3)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [MinLength(5)]
        [MaxLength(20)]
        public string AccountNumber { get; set; } = string.Empty;

        [Range(0, 1000000000)]
        public decimal Balance { get; set; }
    }
}