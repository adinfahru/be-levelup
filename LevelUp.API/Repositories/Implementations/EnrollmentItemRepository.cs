using LevelUp.API.Data;
using LevelUp.API.Entity;
using LevelUp.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LevelUp.API.Repositories.Implementations;

public class EnrollmentItemRepository : Repository<EnrollmentItem>, IEnrollmentItemRepository
{
    public EnrollmentItemRepository(LevelUpDbContext context) : base(context)
    {
    }

    public async Task CreateManyAsync(List<EnrollmentItem> items, CancellationToken cancellationToken)
    {
        await _context.EnrollmentItems.AddRangeAsync(items, cancellationToken);
    }

    public async Task<List<EnrollmentItem>> GetByEnrollmentIdAsync(Guid enrollmentId, CancellationToken cancellationToken)
    {
        return await _context.EnrollmentItems
            .Include(ei => ei.ModuleItem)
            .Where(ei => ei.EnrollmentId == enrollmentId)
            .ToListAsync(cancellationToken);
    }

}
