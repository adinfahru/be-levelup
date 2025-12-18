using LevelUp.API.Data;
using LevelUp.API.Entity;
using LevelUp.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LevelUp.API.Repositories.Implementations;

public class SubmissionRepository : Repository<Submission>, ISubmissionRepository
{
    public SubmissionRepository(LevelUpDbContext context) : base(context)
    {
    }

    public async Task<Submission?> GetByEnrollmentIdAsync(
    Guid enrollmentId,
    CancellationToken cancellationToken)
    {
        return await _context.Submissions
            .FirstOrDefaultAsync(
                s => s.EnrollmentId == enrollmentId,
                cancellationToken
            );
    }
}
