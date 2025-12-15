using LevelUp.API.Data;
using LevelUp.API.Entity;
using LevelUp.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LevelUp.API.Repositories.Implementations;

public class EmployeeRepository : Repository<Employee>, IEmployeeRepository
{
    public EmployeeRepository(LevelUpDbContext context) : base(context)
    {
    }

    public async Task<Employee?> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken)
    {
        return await _context.Employees
            .FirstOrDefaultAsync(e => e.AccountId == accountId, cancellationToken);
    }

    public async Task<Employee?> GetByAccountIdWithRelationsAsync(Guid accountId, CancellationToken cancellationToken)
    {
        return await _context.Employees
            .Include(e => e.Position)
            .FirstOrDefaultAsync(
                e => e.AccountId == accountId,
                cancellationToken);
    }

}
