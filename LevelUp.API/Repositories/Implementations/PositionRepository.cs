using LevelUp.API.Data;
using LevelUp.API.Entity;
using LevelUp.API.Repositories.Interfaces;

namespace LevelUp.API.Repositories.Implementations;

public class PositionRepository : Repository<Position>, IPositionRepository
{
    public PositionRepository(LevelUpDbContext context) : base(context)
    {
    }

}
