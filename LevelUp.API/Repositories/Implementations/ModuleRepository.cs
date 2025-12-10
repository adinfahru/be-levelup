using LevelUp.API.Data;
using LevelUp.API.Entity;
using LevelUp.API.Repositories.Interfaces;

namespace LevelUp.API.Repositories.Implementations;

public class ModuleRepository : Repository<Module>, IModuleRepository
{
    public ModuleRepository(LevelUpDbContext context) : base(context)
    {
    }

}
