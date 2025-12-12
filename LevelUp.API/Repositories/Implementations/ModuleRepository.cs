using LevelUp.API.Data;
using LevelUp.API.Entity;
using LevelUp.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LevelUp.API.Repositories.Implementations;

public class ModuleRepository : Repository<Module>, IModuleRepository
{
    public ModuleRepository(LevelUpDbContext context) : base(context)
    {
    }

    public async Task<int> CountModulesOwnedByManager(Guid managerId)
    {
        return await _context.Modules
            .CountAsync(m => m.CreatedBy == managerId);
    }

    public async Task<int> CountEnrolledEmployeesByManager(Guid managerId)
    {
        return await _context.Enrollments
            .Where(e => e.Module != null && e.Module.CreatedBy == managerId && e.Status == EnrollmentStatus.OnGoing)
            .Select(e => e.AccountId)
            .Distinct()
            .CountAsync();
    }

    public async Task<IEnumerable<Module>> GetModulesByEmployeeId(Guid employeeId)
    {
        return await _context.Modules
            .Where(m => m.Enrollments.Any(e => e.AccountId == employeeId))
            .Include(m => m.Enrollments)
            .ToListAsync();
    }

}
