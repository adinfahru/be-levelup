using LevelUp.API.Data;
using LevelUp.API.Entity;
using LevelUp.API.Repositories.Interfaces;

namespace LevelUp.API.Repositories.Implementations;

public class AccountRepository : Repository<Account>, IAccountRepository
{
    public AccountRepository(LevelUpDbContext context) : base(context)
    {
    }

}
