using LevelUp.API.Entity;

namespace LevelUp.API.Repositories.Interfaces;

public interface IModuleRepository : IRepository<Module>
{
    Task<Module?> GetByIdWithItemsAsync(Guid id, CancellationToken cancellationToken);
}
