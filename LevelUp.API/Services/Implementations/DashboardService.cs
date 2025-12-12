using LevelUp.API.DTOs.Dashboards;
using LevelUp.API.Entity;
using LevelUp.API.Repositories.Interfaces;
using LevelUp.API.Services.Interfaces;

namespace LevelUp.API.Services.Implementations;

public class DashboardService : IDashboardService
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly IModuleRepository _moduleRepository;

    public DashboardService(IEmployeeRepository employeeRepository, IModuleRepository moduleRepository, IAccountRepository accountRepository)
    {
        _employeeRepository = employeeRepository;
        _moduleRepository = moduleRepository;
        _accountRepository = accountRepository;
    }

    public async Task<DashboardResponse> GetDashboardAsync(Guid managerId)
    {
        int totalModules = await _moduleRepository.CountModulesOwnedByManager(managerId);
        int totalEnroll = await _moduleRepository.CountEnrolledEmployeesByManager(managerId);

        var allEmployees = await _employeeRepository.GetAllEmployees();
        int totalIdle = allEmployees.Count(e => e.IsIdle);
        int totalEmployee = allEmployees.Count();

        return new DashboardResponse(totalIdle, totalEnroll, totalModules, totalEmployee);
    }

    public async Task<EmployeeDetailResponse> GetEmployeeDetailAsync(Guid employeeId, Guid managerId)
    {
        var employee = await _employeeRepository.GetEmployeeByIdAsync(employeeId);
        var modules = await _moduleRepository.GetModulesByEmployeeId(employeeId);

        var status = employee.IsIdle ? "Idle" : "Active";

        return new EmployeeDetailResponse(
            employee.Id,
            employee.FirstName ?? string.Empty,
            employee.LastName ?? string.Empty,
            status,
            modules.Select(m => m.Title ?? string.Empty)
        );
    }

    public async Task<IEnumerable<EmployeeListResponse>> GetEmployeesAsync(Guid managerId)
    {
        var allEmployees = await _employeeRepository.GetAllEmployees();
        var enrolledIds = await _moduleRepository.CountEnrolledEmployeesByManager(managerId);

        var filtered = allEmployees
            .Where(e => e.Account != null && e.Account.Role == UserRole.Employee);

        return filtered.Select(e => new EmployeeListResponse(
            e.Id,
            e.FirstName ?? string.Empty,
            e.LastName ?? string.Empty,
            e.Account?.Email ?? string.Empty,
            e.IsIdle ? "Idle" : "Active"
        ));
    }

    public async Task<bool> UpdateEmployeeStatusAsync(Guid employeeId, bool isIdle)
    {
        var employee = await _employeeRepository.GetEmployeeByIdAsync(employeeId);
        employee.IsIdle = isIdle;
        return await _employeeRepository.UpdateAsync(employee);
    }

}
