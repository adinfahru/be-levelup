using LevelUp.API.DTOs.Dashboards;

namespace LevelUp.API.Services.Interfaces;

public interface IDashboardService
{
    Task<DashboardResponse> GetDashboardAsync(Guid managerId);
    Task<IEnumerable<EmployeeListResponse>> GetEmployeesAsync(Guid managerId);
    Task<EmployeeDetailResponse> GetEmployeeDetailAsync(Guid employeeId, Guid managerId);
    Task<bool> UpdateEmployeeStatusAsync(Guid employeeId, bool isIdle);
}
