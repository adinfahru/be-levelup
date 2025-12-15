using LevelUp.API.DTOs.Dashboards;
using LevelUp.API.Entity;

namespace LevelUp.API.Services.Interfaces;

public interface IDashboardService
{
    Task<DashboardResponse> GetDashboardAsync(Guid accountIdFromJwt);
    Task<IEnumerable<EmployeeListResponse>> GetEmployeesAsync(Guid managerAccountId);
    Task<EmployeeDetailResponse> GetEmployeeDetailAsync(
        Guid? employeeId,
        Guid? accountIdFromJwt,
        CancellationToken cancellationToken
    );
    Task<bool> UpdateEmployeeStatusAsync(Guid employeeId, bool isIdle, CancellationToken cancellationToken);
    Task<IEnumerable<EmployeeEnrollResponse>> GetEnrollmentsByManagerId(Guid accountIdFromJwt);
}
