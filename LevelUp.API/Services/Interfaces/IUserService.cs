using LevelUp.API.DTOs.AccountManagement;
using LevelUp.API.Entity;

namespace LevelUp.API.Services.Interfaces;

public interface IUserService
{
    Task CreateAccountAsync(UserRequest request, CancellationToken cancellationToken);
    Task UpdateAccountAsync(Guid accountId, UserRequest request, CancellationToken cancellationToken);
    Task DeleteAccountAsync(Guid accountId, CancellationToken cancellationToken);
    Task<IEnumerable<UserResponse>> GetAllAccountsAsync(CancellationToken cancellationToken);
    Task<UserResponse> GetAccountByIdAsync(Guid accountId, CancellationToken cancellationToken);

}
