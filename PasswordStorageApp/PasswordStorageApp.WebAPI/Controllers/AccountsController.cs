using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PasswordStorageApp.Domain.Dtos;
using PasswordStorageApp.WebAPI.Persistens.Contexts;

namespace PasswordStorageApp.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public AccountsController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
        {
            var accounts =
                await _dbContext
                    .Accounts
                    .AsNoTracking()
                    .Select(ac => AccountGetAllDto.MapFromAccount(ac))
                    .ToListAsync(cancellationToken);
                
            return Ok(accounts);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            if (id == Guid.Empty)
                return BadRequest("Id is not valid");

            var account = await _dbContext
                .Accounts
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (account == null)
                return NotFound();

            return Ok(account);
        }

        [HttpPost]
        public async Task<IActionResult> Create(AccountCreateDto newAccount, CancellationToken cancellationToken)
        {
            var account = newAccount.ToAccount();

            _dbContext.Accounts.Add(account);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Ok(new { data = account.Id, message = "The account was added successfully !" });
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateAsync(Guid id, AccountUpdateDto updateDto, CancellationToken cancellationToken)
        {
            if (id != updateDto.Id)
                return BadRequest("The id in the URL does not match the id in the body");

            var account = _dbContext
                .Accounts
                .FirstOrDefault(ac => ac.Id == id);

            if (account == null)
                return NotFound();

            var updatedAccount = updateDto.ToAccount(account);

            //_dbContext .Accounts.Add(updatedAccount);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Ok(new { data = updatedAccount, message = "The account was updated successfully!" });
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> RemoveAsync(Guid id, CancellationToken cancellationToken)
        {
            if (id == Guid.Empty)
                return BadRequest("Id is not valid");

            var account = _dbContext.Accounts.FirstOrDefault(x => x.Id == id);

            if (account == null)
                return NotFound();

            _dbContext.Accounts.Remove(account);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return NoContent();
        }
    }
}