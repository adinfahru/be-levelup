using LevelUp.API.Entity;

namespace LevelUp.API.Repositories.Interfaces;

public interface IAccountRepository : IRepository<Account>
{
    Task<Account?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Task<IEnumerable<Account>> GetAllWithEmployeesAsync(CancellationToken cancellationToken);
}
