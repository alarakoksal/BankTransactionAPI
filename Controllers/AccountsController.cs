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

        // ✅ GET: /api/Accounts
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

        // ✅ POST: /api/Accounts
        [HttpPost]
        public async Task<ActionResult<AccountResponse>> Create([FromBody] CreateAccountRequest request)
        {
            // Bu satır validation için önemli (ApiController zaten auto validate yapar ama net olsun diye bırakıyoruz)
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Aynı accountNumber ile hesap açılmasını engelleyelim
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

            var response = new AccountResponse
            {
                Id = account.Id,
                FullName = account.FullName,
                AccountNumber = account.AccountNumber,
                Balance = account.Balance
            };

            return Ok(response);
        }
    }
}