using LevelUp.API.Data;
using LevelUp.API.Entity;
using LevelUp.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LevelUp.API.Repositories.Implementations;

public class ModuleRepository(LevelUpDbContext context) : Repository<Module>(context), IModuleRepository
{

    public async Task<Module?> GetByIdWithItemsAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Modules
            .Include(m => m.Items)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
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

    public async Task<Module?> GetByIdWithCreatorAsync(
        Guid moduleId,
        CancellationToken cancellationToken)
    {
        return await _context.Modules
            .Include(m => m.Creator)
            .FirstOrDefaultAsync(m => m.Id == moduleId, cancellationToken);
    }
}
