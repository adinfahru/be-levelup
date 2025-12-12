using LevelUp.API.DTOs.AccountManagement;
using LevelUp.API.Entity;
using LevelUp.API.Repositories.Interfaces;
using LevelUp.API.Services.Interfaces;

namespace LevelUp.API.Services.Implementations;

public class UserService : IUserService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UserService(IAccountRepository accountRepository, IEmployeeRepository employeeRepository, IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _employeeRepository = employeeRepository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task CreateAccountAsync(UserRequest request, CancellationToken cancellationToken)
    {
        var accountId = Guid.NewGuid();

        var account = new Account
        {
            Id = accountId,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = request.Role,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
        };

        var employee = new Employee
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            IsIdle = false,
            CreatedAt = DateTime.UtcNow,

            // Relasi ke account
            AccountId = accountId,
            Account = account,

            PositionId = request.PositionId
        };

        await _unitOfWork.CommitTransactionAsync(async () =>
        {
            await _employeeRepository.CreateAsync(employee, cancellationToken);
            await _accountRepository.CreateAsync(account, cancellationToken);
        }, cancellationToken);
    }

    public async Task UpdateAccountAsync(Guid accountId, UserRequest request, CancellationToken cancellationToken)
    {
        // Ambil account
        var account = await _accountRepository.GetByIdAsync(accountId, cancellationToken);

        if (account is null)
            throw new NullReferenceException("Account not found");

        // Ambil employee berdasarkan AccountId
        var employee = await _employeeRepository.GetByAccountIdAsync(accountId, cancellationToken);

        if (employee is null)
            throw new NullReferenceException("Employee not found for this account");

        // Update Account
        account.Email = request.Email;
        account.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        account.Role = request.Role;
        account.UpdatedAt = DateTime.UtcNow;

        // Update Employee
        employee.FirstName = request.FirstName;
        employee.LastName = request.LastName;
        employee.PositionId = request.PositionId;
        employee.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.CommitTransactionAsync(async () =>
        {
            await _accountRepository.UpdateAsync(account);
            await _employeeRepository.UpdateAsync(employee);
        }, cancellationToken);
    }

    public async Task DeleteAccountAsync(Guid accountId, CancellationToken cancellationToken)
    {
        var account = await _accountRepository.GetByIdAsync(accountId, cancellationToken);

        if (account == null)
            throw new Exception("Account not found");

        // Soft delete account
        account.IsActive = false;

        await _unitOfWork.CommitTransactionAsync(async () =>
        {
            await _accountRepository.UpdateAsync(account);
        }, cancellationToken);
    }
    public async Task<UserResponse> GetAccountByIdAsync(Guid accountId, CancellationToken cancellationToken)
    {
        var account = await _accountRepository.GetByIdAsync(accountId, cancellationToken);

        if (account is null)
            throw new NullReferenceException("Account not found");

        // Ambil employee berdasarkan AccountId
        var employee = await _employeeRepository.GetByAccountIdAsync(accountId, cancellationToken);

        if (employee is null)
            throw new NullReferenceException("Employee not found for this account");

        return new UserResponse(
            account.Id,
            account.Email!,
            account.Role,
            account.IsActive,
            employee.FirstName ?? "",
            employee.LastName ?? "",
            employee.PositionId,
            account.CreatedAt,
            account.UpdatedAt
        );
    }

    public async Task<IEnumerable<UserResponse>> GetAllAccountsAsync(CancellationToken cancellationToken)
    {
        var accounts = await _accountRepository.GetAllWithEmployeesAsync(cancellationToken);

        if (!accounts.Any())
            throw new NullReferenceException("No accounts found");

        var accountList = accounts.Select(account => 
            new UserResponse(
                account.Id,
                account.Email!,
                account.Role,
                account.IsActive,
                account.Employee?.FirstName ?? "",
                account.Employee?.LastName ?? "",
                account.Employee?.PositionId,
                account.CreatedAt,
                account.UpdatedAt
            )
        );

        return accountList;
    }
}
