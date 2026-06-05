using BankTransactionTracker.Data;
using BankTransactionTracker.DTOs;
using BankTransactionTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BankTransactionTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AccountsController(AppDbContext context)
        {
            _context = context;
        }

        //  GET: /api/Accounts
        [HttpGet]
        public async Task<ActionResult<List<AccountResponse>>> GetAll()
        {
            var accounts = await _context.Accounts
                .Select(a => new AccountResponse
                {
                    Id = a.Id,
                    FullName = a.FullName,
                    AccountNumber = a.AccountNumber,
                    Balance = a.Balance
                })
                .ToListAsync();

            return Ok(accounts);
        }

        // GET: /api/Accounts/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<AccountResponse>> GetById(int id)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
                return NotFound();

            return Ok(new AccountResponse
            {
                Id = account.Id,
                FullName = account.FullName,
                AccountNumber = account.AccountNumber,
                Balance = account.Balance
            });
        }

        // POST: /api/Accounts
        [HttpPost]
        public async Task<ActionResult<AccountResponse>> Create([FromBody] CreateAccountRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var exists = await _context.Accounts.AnyAsync(a => a.AccountNumber == request.AccountNumber);
            if (exists)
                return BadRequest("AccountNumber already exists.");

            var account = new Account
            {
                FullName = request.FullName,
                AccountNumber = request.AccountNumber,
                Balance = request.Balance
            };

            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            return Ok(new AccountResponse
            {
                Id = account.Id,
                FullName = account.FullName,
                AccountNumber = account.AccountNumber,
                Balance = account.Balance
            });
        }

        // PUT: /api/Accounts/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<AccountResponse>> Update(int id, [FromBody] CreateAccountRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
                return NotFound();

            var duplicate = await _context.Accounts
                .AnyAsync(a => a.AccountNumber == request.AccountNumber && a.Id != id);
            if (duplicate)
                return BadRequest("AccountNumber already exists.");

            account.FullName = request.FullName;
            account.AccountNumber = request.AccountNumber;

            await _context.SaveChangesAsync();

            return Ok(new AccountResponse
            {
                Id = account.Id,
                FullName = account.FullName,
                AccountNumber = account.AccountNumber,
                Balance = account.Balance
            });
        }

        // GET: /api/Accounts/{id}/transactions
        [HttpGet("{id}/transactions")]
        public async Task<ActionResult<List<TransactionResponse>>> GetTransactions(int id)
        {
            var exists = await _context.Accounts.AnyAsync(a => a.Id == id);
            if (!exists)
                return NotFound();

            var transactions = await _context.Transactions
                .Where(t => t.SenderAccountId == id || t.ReceiverAccountId == id)
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

        // DELETE: /api/Accounts/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
                return NotFound();

            var hasTransactions = await _context.Transactions
                .AnyAsync(t => t.SenderAccountId == id || t.ReceiverAccountId == id);
            if (hasTransactions)
                return BadRequest("Cannot delete account with existing transactions.");

            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}