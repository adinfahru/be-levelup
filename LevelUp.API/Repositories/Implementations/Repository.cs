using System.Linq.Expressions;
using LevelUp.API.Data;
using LevelUp.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LevelUp.API.Repositories.Implementations;

public class Repository<Repo> : IRepository<Repo> where Repo : class
{

    protected readonly LevelUpDbContext _context;
    // public Repository() : base(repo)
    public Repository(LevelUpDbContext context)
    {
        _context = context;
    }
    public async Task CreateAsync(Repo param, CancellationToken cancellationToken)
    {
        await _context.Set<Repo>().AddAsync(param, cancellationToken);
    }

    public Task DeleteAsync(Repo param)
    {
        _context.Set<Repo>().Remove(param);
        return Task.CompletedTask;
    }

    public async Task<IEnumerable<Repo>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Set<Repo>().ToListAsync(cancellationToken);
    }

    public async Task<Repo?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Set<Repo>().FindAsync(id, cancellationToken);
    }

    public Task UpdateAsync(Repo param)
    {
        _context.Set<Repo>().Update(param);
        return Task.CompletedTask;
    }

    public async Task<Repo?> FirstOrDefaultAsync(Expression<Func<Repo, bool>> predicate)
    {
        return await _context.Set<Repo>().FirstOrDefaultAsync(predicate);
    }

    public IQueryable<Repo> GetQueryable()
    {
        return _context.Set<Repo>().AsQueryable();
    }
}
