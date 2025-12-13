using System.Linq.Expressions;

namespace LevelUp.API.Repositories.Interfaces;

public interface IRepository<Repo>
{
    Task<IEnumerable<Repo>> GetAllAsync(CancellationToken cancellationToken);
    Task<Repo?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task CreateAsync(Repo param, CancellationToken cancellationToken);
    Task UpdateAsync(Repo param);
    Task DeleteAsync(Repo param);
    Task<Repo?> FirstOrDefaultAsync(Expression<Func<Repo, bool>> predicate);
    IQueryable<Repo> GetQueryable();
}
