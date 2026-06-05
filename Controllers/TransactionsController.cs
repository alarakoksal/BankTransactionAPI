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

        [HttpGet]
        public async Task<ActionResult<List<TransactionResponse>>> GetAll()
        {
            var transactions = await _context.Transactions
                .OrderByDescending(t => t.Date)
                .Join(_context.Accounts, t => t.SenderAccountId, a => a.Id, (t, sender) => new { t, sender })
                .Join(_context.Accounts, ts => ts.t.ReceiverAccountId, a => a.Id, (ts, receiver) => new TransactionResponse
                {
                    Id = ts.t.Id,
                    SenderAccountId = ts.t.SenderAccountId,
                    SenderName = ts.sender.FullName,
                    SenderAccountNumber = ts.sender.AccountNumber,
                    ReceiverAccountId = ts.t.ReceiverAccountId,
                    ReceiverName = receiver.FullName,
                    ReceiverAccountNumber = receiver.AccountNumber,
                    Amount = ts.t.Amount,
                    Date = ts.t.Date
                })
                .ToListAsync();

            return Ok(transactions);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TransactionResponse>> GetById(int id)
        {
            var result = await _context.Transactions
                .Where(t => t.Id == id)
                .Join(_context.Accounts, t => t.SenderAccountId, a => a.Id, (t, sender) => new { t, sender })
                .Join(_context.Accounts, ts => ts.t.ReceiverAccountId, a => a.Id, (ts, receiver) => new TransactionResponse
                {
                    Id = ts.t.Id,
                    SenderAccountId = ts.t.SenderAccountId,
                    SenderName = ts.sender.FullName,
                    SenderAccountNumber = ts.sender.AccountNumber,
                    ReceiverAccountId = ts.t.ReceiverAccountId,
                    ReceiverName = receiver.FullName,
                    ReceiverAccountNumber = receiver.AccountNumber,
                    Amount = ts.t.Amount,
                    Date = ts.t.Date
                })
                .FirstOrDefaultAsync();

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("filter")]
        public async Task<ActionResult<List<TransactionResponse>>> Filter(
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
                .Join(_context.Accounts, t => t.SenderAccountId, a => a.Id, (t, sender) => new { t, sender })
                .Join(_context.Accounts, ts => ts.t.ReceiverAccountId, a => a.Id, (ts, receiver) => new TransactionResponse
                {
                    Id = ts.t.Id,
                    SenderAccountId = ts.t.SenderAccountId,
                    SenderName = ts.sender.FullName,
                    SenderAccountNumber = ts.sender.AccountNumber,
                    ReceiverAccountId = ts.t.ReceiverAccountId,
                    ReceiverName = receiver.FullName,
                    ReceiverAccountNumber = receiver.AccountNumber,
                    Amount = ts.t.Amount,
                    Date = ts.t.Date
                })
                .ToListAsync();

            return Ok(result);
        }

        [HttpPost("transfer")]
        public async Task<ActionResult> Transfer([FromBody] TransferRequest request)
        {
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

            using var dbTransaction = await _context.Database.BeginTransactionAsync();
            try
            {
                sender.Balance -= request.Amount;
                receiver.Balance += request.Amount;

                var tx = new Transaction
                {
                    SenderAccountId = request.SenderAccountId,
                    ReceiverAccountId = request.ReceiverAccountId,
                    Amount = request.Amount,
                    Date = DateTime.UtcNow
                };

                _context.Transactions.Add(tx);
                await _context.SaveChangesAsync();
                await dbTransaction.CommitAsync();

                return Ok(new TransactionResponse
                {
                    Id = tx.Id,
                    SenderAccountId = sender.Id,
                    SenderName = sender.FullName,
                    SenderAccountNumber = sender.AccountNumber,
                    ReceiverAccountId = receiver.Id,
                    ReceiverName = receiver.FullName,
                    ReceiverAccountNumber = receiver.AccountNumber,
                    Amount = tx.Amount,
                    Date = tx.Date
                });
            }
            catch
            {
                await dbTransaction.RollbackAsync();
                throw;
            }
        }
    }
}
