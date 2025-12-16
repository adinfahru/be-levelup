using LevelUp.API.Data;
using LevelUp.API.DTOs.Dashboards;
using LevelUp.API.Entity;
using LevelUp.API.Repositories.Interfaces;
using LevelUp.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LevelUp.API.Services.Implementations;

public class DashboardService : IDashboardService
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IModuleRepository _moduleRepository;
    private readonly LevelUpDbContext _context;

    public DashboardService(
        IEmployeeRepository employeeRepository, 
        IModuleRepository moduleRepository, 
        LevelUpDbContext context)
    {
        _employeeRepository = employeeRepository;
        _moduleRepository = moduleRepository;
        _context = context;
    }

    /// <summary>
    /// Get summary dashboard for manager
    /// </summary>
    public async Task<DashboardResponse> GetDashboardAsync(Guid accountIdFromJwt)
    {
        // Ambil employee yang terkait dengan accountId
        var managerEmployee = await _employeeRepository
            .GetByAccountIdAsync(accountIdFromJwt, CancellationToken.None);

        if (managerEmployee == null)
            throw new Exception("Manager not found");

        var managerEmployeeId = managerEmployee.Id;

        // Hitung total modules yang dibuat manager
        int totalModules = await _context.Modules
            .CountAsync(m => m.CreatedBy == managerEmployeeId);

        // Hitung total enrollments dari modul yang dibuat manager
        int totalEnroll = await _context.Enrollments
            .Include(e => e.Module)
            .CountAsync(e => e.Module != null && e.Module.CreatedBy == managerEmployeeId);

        // Hitung total employee & idle
        var allEmployees = await _employeeRepository.GetAllEmployees();
        int totalIdle = allEmployees.Count(e => e.IsIdle);
        int totalEmployee = allEmployees.Count();

        return new DashboardResponse(totalIdle, totalEnroll, totalModules, totalEmployee);
    }

    // Employee detail, bisa pakai employeeId atau fallback ke accountId
    public async Task<EmployeeDetailResponse> GetEmployeeDetailAsync(
        Guid? employeeId,
        Guid? accountIdFromJwt,
        CancellationToken cancellationToken)
    {
        Employee? employee = null;

        if (employeeId.HasValue)
            employee = await _employeeRepository.GetByIdWithAccountAsync(employeeId.Value, cancellationToken);
        else if (accountIdFromJwt.HasValue)
            employee = await _employeeRepository.GetByAccountIdAsync(accountIdFromJwt.Value, cancellationToken);

        if (employee == null || employee.Account == null)
            throw new Exception("Employee not found");

        return new EmployeeDetailResponse(
            Id: employee.Id,
            FirstName: employee.FirstName ?? "",
            LastName: employee.LastName ?? "",
            IsIdle: employee.IsIdle,
            Email: employee.Account.Email ?? "",
            Role: employee.Account.Role.ToString(),
            PositionName: employee.Position?.Title
        );
    }

    // List semua employee
    public async Task<IEnumerable<EmployeeListResponse>> GetEmployeesAsync(Guid managerAccountId)
    {
        var allEmployees = await _employeeRepository.GetAllEmployees();
        var filtered = allEmployees.Where(e => e.Account != null && e.Account.Role == UserRole.Employee);

        return filtered.Select(e => new EmployeeListResponse(
            e.Id,
            e.FirstName ?? string.Empty,
            e.LastName ?? string.Empty,
            e.Account?.Email ?? string.Empty,
            e.IsIdle
        ));
    }

    // Update status idle employee
    public async Task<bool> UpdateEmployeeStatusAsync(Guid employeeId, bool isIdle, CancellationToken cancellationToken)
    {
        var employee = await _context.Employees
            .Include(e => e.Account)
            .FirstOrDefaultAsync(e => e.Id == employeeId, cancellationToken);

        if (employee == null)
            throw new Exception("Employee not found");

        employee.IsIdle = isIdle;

        return await _context.SaveChangesAsync(cancellationToken) > 0;
    }

    // Enrollments berdasarkan manager
    public async Task<IEnumerable<EmployeeEnrollResponse>> GetEnrollmentsByManagerId(Guid accountIdFromJwt)
    {
        // Ambil employee manager
        var managerEmployee = await _employeeRepository
            .GetByAccountIdAsync(accountIdFromJwt, CancellationToken.None);

        if (managerEmployee == null)
            throw new Exception("Manager not found");

        var managerEmployeeId = managerEmployee.Id;

        var enrollments = await _context.Enrollments
            .Include(e => e.Account).ThenInclude(a => a.Employee)
            .Include(e => e.Module)
            .Where(e => e.Module != null && e.Module.CreatedBy == managerEmployeeId)
            .ToListAsync();

        return enrollments
            .Where(e => e.Account?.Employee != null)
            .Select(e => new EmployeeEnrollResponse(
                e.Account.Employee.Id,
                e.Account.Employee.FirstName ?? "",
                e.Account.Employee.LastName ?? "",
                e.Account.Email ?? "",
                e.Account.Employee.IsIdle,
                e.Status.ToString()
            ))
            .ToList();
    }
}
