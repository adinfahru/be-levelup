using LevelUp.API.Entity;

namespace LevelUp.API.Repositories.Interfaces;

public interface IEmployeeRepository : IRepository<Employee>
{
    Task<Employee?> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken);
    Task<Employee?> GetByAccountIdWithRelationsAsync(Guid accountId, CancellationToken cancellationToken);
    Task<Employee?> GetByIdWithAccountAsync(Guid employeeId, CancellationToken cancellationToken);
    Task<IEnumerable<Employee>> GetAllEmployees();
    Task<bool> UpdateAsync(Employee employee, CancellationToken cancellationToken);
}
