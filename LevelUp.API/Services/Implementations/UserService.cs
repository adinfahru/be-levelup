using LevelUp.API.DTOs.AccountManagement;
using LevelUp.API.Entity;
using LevelUp.API.Repositories.Interfaces;
using LevelUp.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LevelUp.API.Services.Implementations;

public class UserService : IUserService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IPositionRepository _positionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UserService(IAccountRepository accountRepository, IEmployeeRepository employeeRepository, IPositionRepository positionRepository, IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _employeeRepository = employeeRepository;
        _positionRepository = positionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task CreateAccountAsync(UserRequest request, CancellationToken cancellationToken)
    {
        // Validate position if provided
        if (request.PositionId.HasValue)
        {
            var position = await _positionRepository.GetByIdAsync(request.PositionId.Value, cancellationToken);
            if (position == null)
                throw new InvalidOperationException($"Position with ID '{request.PositionId}' not found");
        }

        var accountId = Guid.NewGuid();

        var account = new Account
        {
            Id = accountId,
            Email = request.Email,
            PasswordHash = string.IsNullOrWhiteSpace(request.Password)
                ? throw new ArgumentException("Password is required for creating an account")
                : BCrypt.Net.BCrypt.HashPassword(request.Password),
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
        // Validate position if provided
        if (request.PositionId.HasValue)
        {
            var position = await _positionRepository.GetByIdAsync(request.PositionId.Value, cancellationToken);
            if (position == null)
                throw new InvalidOperationException($"Position with ID '{request.PositionId}' not found");
        }

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
        // Only update password when a new password is provided
        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            account.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        }
        account.Role = request.Role;
        if (request.IsActive.HasValue)
            account.IsActive = request.IsActive.Value;
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

        // Ambil employee berdasarkan AccountId dengan Position
        var employee = await _employeeRepository.GetByAccountIdAsync(accountId, cancellationToken);

        if (employee is null)
            throw new NullReferenceException("Employee not found for this account");

        // Load Position if PositionId exists
        Position? position = null;
        if (employee.PositionId.HasValue)
        {
            position = await _positionRepository.GetByIdAsync(employee.PositionId.Value, cancellationToken);
        }

        return new UserResponse(
            account.Id,
            account.Email!,
            account.Role,
            account.IsActive,
            employee.FirstName ?? "",
            employee.LastName ?? "",
            employee.PositionId,
            position?.Title,
            account.CreatedAt,
            account.UpdatedAt
        );
    }

    public async Task<(IEnumerable<UserResponse> items, int total)> GetAllAccountsAsync(
        int page,
        int limit,
        string? role,
        string? search,
        bool? isActive,
        CancellationToken cancellationToken)
    {
        IQueryable<Account> query = _accountRepository.GetQueryable()
            .Include(a => a.Employee)
                .ThenInclude(e => e!.Position);

        // Filter by isActive
        if (isActive.HasValue)
            query = query.Where(a => a.IsActive == isActive.Value);

        // Filter by role (parse string to enum to keep translation server-side)
        if (!string.IsNullOrWhiteSpace(role) && Enum.TryParse<UserRole>(role, true, out var parsedRole))
        {
            query = query.Where(a => a.Role == parsedRole);
        }

        // Filter by search (email, firstName, lastName) — use SQL-friendly LIKE via ToLower()
        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLower();
            query = query.Where(a =>
                (a.Email != null && a.Email.ToLower().Contains(s)) ||
                (a.Employee != null && a.Employee.FirstName != null && a.Employee.FirstName.ToLower().Contains(s)) ||
                (a.Employee != null && a.Employee.LastName != null && a.Employee.LastName.ToLower().Contains(s))
            );
        }

        var total = await query.CountAsync(cancellationToken);
        var accounts = await query
            .OrderByDescending(a => a.CreatedAt)
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync(cancellationToken);

        var accountList = accounts.Select(account =>
            new UserResponse(
                account.Id,
                account.Email!,
                account.Role,
                account.IsActive,
                account.Employee?.FirstName ?? "",
                account.Employee?.LastName ?? "",
                account.Employee?.PositionId,
                account.Employee?.Position?.Title,
                account.CreatedAt,
                account.UpdatedAt
            )
        );

        return (accountList.AsEnumerable(), total);
    }
}
