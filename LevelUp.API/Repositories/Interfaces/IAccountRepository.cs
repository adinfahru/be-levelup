using LevelUp.API.Entity;

namespace LevelUp.API.Repositories.Interfaces;

public interface IAccountRepository : IRepository<Account>
{
    Task<IEnumerable<Account>> GetAllWithEmployeesAsync(CancellationToken cancellationToken);
}
