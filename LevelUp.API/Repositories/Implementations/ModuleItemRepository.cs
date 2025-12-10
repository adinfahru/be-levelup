using LevelUp.API.Data;
using LevelUp.API.Entity;
using LevelUp.API.Repositories.Interfaces;

namespace LevelUp.API.Repositories.Implementations;

public class ModuleItemRepository : Repository<ModuleItem>, IModuleItemRepository
{
    public ModuleItemRepository(LevelUpDbContext context) : base(context)
    {
    }

}
