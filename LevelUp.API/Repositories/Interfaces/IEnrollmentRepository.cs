using LevelUp.API.Entity;

namespace LevelUp.API.Repositories.Interfaces;

public interface IEnrollmentRepository : IRepository<Enrollment>
{
    Task<Enrollment?> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<List<Enrollment>> GetCompletedByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<Enrollment?> GetByIdAndAccountIdAsync(Guid enrollmentId, Guid accountId, CancellationToken cancellationToken);
    Task<bool> HasCompletedModuleAsync(Guid accountId,Guid moduleId,CancellationToken cancellationToken);


}
