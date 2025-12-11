using LevelUp.API.DTOs.AccountManagement;
using LevelUp.API.Entity;
using LevelUp.API.Repositories.Interfaces;
using LevelUp.API.Services.Interfaces;

namespace LevelUp.API.Services.Implementations;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AccountService(IAccountRepository accountRepository, IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task CreateAccountAsync(AccountRequest request, CancellationToken cancellationToken)
    {
        var createAccount = new Account
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = request.Role,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        await _unitOfWork.CommitTransactionAsync(async () =>
        {
            await _accountRepository.CreateAsync(createAccount, cancellationToken);
        }, cancellationToken);
    }

    public async Task UpdateAccountAsync(Guid accountId, AccountRequest request, CancellationToken cancellationToken)
    {
        var account = await _accountRepository.GetByIdAsync(accountId, cancellationToken);
        
        if(account is null)
        {
            throw new NullReferenceException("Account not found");
        }

        // Update fields langsung pada entity yang sudah ditrack EF
        account.Email = request.Email;
        account.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        account.Role = request.Role;
        account.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.CommitTransactionAsync(async () =>
        {
            await _accountRepository.UpdateAsync(account); // sekarang tidak error
        }, cancellationToken);
    }

    public async Task DeleteAccountAsync(Guid accountId, CancellationToken cancellationToken)
    {
        var account = await _accountRepository.GetByIdAsync(accountId, cancellationToken);

        if(account is null)
        {
            throw new NullReferenceException("Account not found");
        }

        await _unitOfWork.CommitTransactionAsync(async () =>
        {
            await _accountRepository.DeleteAsync(account);
        }, cancellationToken);
    }
    public async Task<AccountResponse> GetAccountByIdAsync(Guid accountId, CancellationToken cancellationToken)
    {
        var account = await _accountRepository.GetByIdAsync(accountId, cancellationToken);
        
        if(account is null)
        {
            throw new NullReferenceException("Account not found");
        }

        var accountMap = new AccountResponse
        (
            account.Id,
            account.Email,
            account.Role,
            account.IsActive,
            account.CreatedAt,
            account.UpdatedAt
        );

        return accountMap;
    }

    public async Task<IEnumerable<AccountResponse>> GetAllAccountsAsync(CancellationToken cancellationToken)
    {
        var accounts = await _accountRepository.GetAllAsync(cancellationToken);

        if(!accounts.Any())
        {
            throw new NullReferenceException("No accounts found");
        }

        var accountMaps = accounts.Select(account => new AccountResponse
        (
            account.Id,
            account.Email,
            account.Role,
            account.IsActive,
            account.CreatedAt,
            account.UpdatedAt
        ));

        return accountMaps;
    }
}
