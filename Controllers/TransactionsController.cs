using BankTransactionTracker.Data;
using BankTransactionTracker.DTOs;
using BankTransactionTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BankTransactionTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TransactionsController(AppDbContext context)
        {
            _context = context;
        }

        // Listeleme
        [HttpGet]
        public async Task<ActionResult<List<Transaction>>> GetAll()
        {
            var transactions = await _context.Transactions
                .OrderByDescending(t => t.Date)
                .ToListAsync();

            return Ok(transactions);
        }

        // Filtreleme
        [HttpGet("filter")]
        public async Task<ActionResult<List<Transaction>>> Filter(
            [FromQuery] decimal? minAmount,
            [FromQuery] decimal? maxAmount,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            var query = _context.Transactions.AsQueryable();

            if (minAmount.HasValue)
                query = query.Where(t => t.Amount >= minAmount.Value);

            if (maxAmount.HasValue)
                query = query.Where(t => t.Amount <= maxAmount.Value);

            if (startDate.HasValue)
                query = query.Where(t => t.Date >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(t => t.Date <= endDate.Value);

            var result = await query
                .OrderByDescending(t => t.Date)
                .ToListAsync();

            return Ok(result);
        }

        // Transfer
        [HttpPost("transfer")]
        public async Task<ActionResult> Transfer([FromBody] TransferRequest request)
        {
            // DTO validation (ApiController bunu otomatik yapar ama net olsun)
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (request.SenderAccountId == request.ReceiverAccountId)
                return BadRequest("Sender and receiver cannot be the same.");

            var sender = await _context.Accounts.FindAsync(request.SenderAccountId);
            var receiver = await _context.Accounts.FindAsync(request.ReceiverAccountId);

            if (sender == null || receiver == null)
                return NotFound("Sender or receiver account not found.");

            if (sender.Balance < request.Amount)
                return BadRequest("Insufficient balance.");

            // Güncelleme
            sender.Balance -= request.Amount;
            receiver.Balance += request.Amount;

            // Kayıt
            var tx = new Transaction
            {
                SenderAccountId = request.SenderAccountId,
                ReceiverAccountId = request.ReceiverAccountId,
                Amount = request.Amount,
                Date = DateTime.UtcNow
            };

            _context.Transactions.Add(tx);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Transfer successful.",
                Transaction = tx,
                SenderBalance = sender.Balance,
                ReceiverBalance = receiver.Balance
            });
        }
    }
}