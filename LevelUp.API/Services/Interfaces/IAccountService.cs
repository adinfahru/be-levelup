using LevelUp.API.DTOs.AccountManagement;
using LevelUp.API.Entity;

namespace LevelUp.API.Services.Interfaces;

public interface IAccountService
{
    Task CreateAccountAsync(AccountRequest request, CancellationToken cancellationToken);
    Task UpdateAccountAsync(Guid accountId, AccountRequest request, CancellationToken cancellationToken);
    Task DeleteAccountAsync(Guid accountId, CancellationToken cancellationToken);
    Task<IEnumerable<AccountResponse>> GetAllAccountsAsync(CancellationToken cancellationToken);
    Task<AccountResponse> GetAccountByIdAsync(Guid accountId, CancellationToken cancellationToken);

}
