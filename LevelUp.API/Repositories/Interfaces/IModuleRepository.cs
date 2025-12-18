using LevelUp.API.Entity;

namespace LevelUp.API.Repositories.Interfaces;

public interface IModuleRepository : IRepository<Module>
{
    Task<Module?> GetByIdWithItemsAsync(Guid id, CancellationToken cancellationToken);
    Task<int> CountModulesOwnedByManager(Guid managerId);
    Task<int> CountEnrolledEmployeesByManager(Guid managerId);
    Task<IEnumerable<Module>> GetModulesByEmployeeId(Guid employeeId);
    Task<Module?> GetByIdWithCreatorAsync(Guid moduleId, CancellationToken cancellationToken);
}
