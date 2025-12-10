using LevelUp.API.Entity;

namespace LevelUp.API.Repositories.Interfaces;

public interface IUnitOfWork
{
    Task CommitTransactionAsync(Func<Task> action, CancellationToken cancellationToken);
    Task ClearTracksAsync(CancellationToken cancellationToken);
}
