using LevelUp.API.Data;
using LevelUp.API.Entity;
using LevelUp.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LevelUp.API.Repositories.Implementations;

public class EnrollmentRepository : Repository<Enrollment>, IEnrollmentRepository
{
    public EnrollmentRepository(LevelUpDbContext context) : base(context)
    {
    }

    public async Task<Enrollment?> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _context.Enrollments
            .Where(e =>
                e.AccountId == userId &&
                e.Status == EnrollmentStatus.OnGoing)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<Enrollment>> GetCompletedByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _context.Enrollments
            .Where(e =>
                e.AccountId == userId &&
                e.Status == EnrollmentStatus.Completed)
            .OrderByDescending(e => e.CompletedDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<Enrollment?> GetByIdAndAccountIdAsync(Guid enrollmentId, Guid accountId, CancellationToken cancellationToken)
    {
        return await _context.Enrollments
            .Where(e =>
                e.Id == enrollmentId &&
                e.AccountId == accountId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> HasCompletedModuleAsync(
    Guid accountId,
    Guid moduleId,
    CancellationToken cancellationToken)
    {
        return await _context.Enrollments.AnyAsync(e =>
            e.AccountId == accountId &&
            e.ModuleId == moduleId &&
            e.Status == EnrollmentStatus.Completed,
            cancellationToken);
    }
}
