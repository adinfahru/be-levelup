using LevelUp.API.Data;
using LevelUp.API.Entity;
using LevelUp.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LevelUp.API.Repositories.Implementations;

public class ModuleRepository : Repository<Module>, IModuleRepository
{
    public ModuleRepository(LevelUpDbContext context) : base(context)
    {
    }

    public async Task<Module?> GetByIdWithItemsAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Modules
            .Include(m => m.Items)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

}
