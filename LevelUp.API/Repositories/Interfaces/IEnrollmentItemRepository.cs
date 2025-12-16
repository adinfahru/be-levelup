using LevelUp.API.Entity;

namespace LevelUp.API.Repositories.Interfaces;

public interface IEnrollmentItemRepository : IRepository<EnrollmentItem>
{
    Task CreateManyAsync(List<EnrollmentItem> items, CancellationToken cancellationToken);
    Task<List<EnrollmentItem>> GetByEnrollmentIdAsync(Guid enrollmentId, CancellationToken cancellationToken);
}
