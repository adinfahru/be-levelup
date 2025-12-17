using LevelUp.API.Entity;

namespace LevelUp.API.Repositories.Interfaces;

public interface ISubmissionRepository : IRepository<Submission>
{
    Task<Submission?> GetByEnrollmentIdAsync(Guid enrollmentId,CancellationToken cancellationToken);
}
