using LevelUp.API.Data;
using LevelUp.API.Entity;
using LevelUp.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LevelUp.API.Repositories.Implementations;

public class AccountRepository : Repository<Account>, IAccountRepository
{
    public AccountRepository(LevelUpDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Account>> GetAllWithEmployeesAsync(CancellationToken cancellationToken)
    {
        return await _context.Accounts
            .Include(a => a.Employee)
            .ToListAsync(cancellationToken);
    }

}
