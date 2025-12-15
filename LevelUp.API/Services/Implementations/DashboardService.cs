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

    public async Task<DashboardResponse> GetDashboardAsync(Guid managerId)
    {
        int totalModules = await _moduleRepository.CountModulesOwnedByManager(managerId);
        int totalEnroll = await _moduleRepository.CountEnrolledEmployeesByManager(managerId);

        var allEmployees = await _employeeRepository.GetAllEmployees();
        int totalIdle = allEmployees.Count(e => e.IsIdle);
        int totalEmployee = allEmployees.Count();

        return new DashboardResponse(totalIdle, totalEnroll, totalModules, totalEmployee);
    }

    public async Task<EmployeeDetailResponse> GetEmployeeDetailAsync(Guid employeeId, Guid managerId, CancellationToken cancellationToken)
    {
        var employee = await _employeeRepository.GetByIdWithAccountAsync(employeeId, cancellationToken);
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

    public async Task<IEnumerable<EmployeeListResponse>> GetEmployeesAsync(Guid managerId)
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

   public async Task<bool> UpdateEmployeeStatusAsync(Guid employeeId, bool isIdle, CancellationToken cancellationToken)
{
    // Ambil employee langsung dari DbContext agar tracked
    var employee = await _context.Employees
        .Include(e => e.Account) // optional, kalau butuh Account
        .FirstOrDefaultAsync(e => e.Id == employeeId, cancellationToken);

    if (employee == null)
        throw new Exception("Employee not found");

    employee.IsIdle = isIdle;

    // EF Core sudah tracked, jadi SaveChangesAsync cukup
    var updated = await _context.SaveChangesAsync(cancellationToken) > 0;

    return updated;
}


    public async Task<IEnumerable<EmployeeEnrollResponse>> GetEnrollmentsByManagerId(Guid managerId)
    {
        var enrollments = await _context.Enrollments
            .Include(e => e.Account).ThenInclude(a => a.Employee)
            .Include(e => e.Module)
            .Where(e => e.Module != null && e.Module.CreatedBy == managerId)
            .ToListAsync();

        return enrollments
            .Where(e => e.Account?.Employee != null)
            .Select(e => new
            {
                Enrollment = e,
                Employee = e.Account!.Employee!, // ! karena sudah di-filter
                Email = e.Account!.Email ?? ""
            })
            .GroupBy(x => x.Employee.Id)
            .Select(g => new EmployeeEnrollResponse(
                g.Key,
                g.First().Employee.FirstName ?? "",
                g.First().Employee.LastName ?? "",
                g.First().Email,
                g.First().Employee.IsIdle,
                g.First().Enrollment.Status.ToString()
            ))
            .ToList();
    }
}
