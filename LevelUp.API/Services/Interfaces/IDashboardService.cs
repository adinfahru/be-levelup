using LevelUp.API.DTOs.Dashboards;
using LevelUp.API.Entity;

namespace LevelUp.API.Services.Interfaces;

public interface IDashboardService
{
    Task<DashboardResponse> GetDashboardAsync(Guid managerId);
    Task<IEnumerable<EmployeeListResponse>> GetEmployeesAsync(Guid managerId);
    Task<EmployeeDetailResponse> GetEmployeeDetailAsync(Guid employeeId, Guid managerId, CancellationToken cancellationToken);
    Task<bool> UpdateEmployeeStatusAsync(Guid employeeId, bool isIdle, CancellationToken cancellationToken);
    Task<IEnumerable<EmployeeEnrollResponse>> GetEnrollmentsByManagerId(Guid managerId);
}
