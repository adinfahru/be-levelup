using LevelUp.API.DTOs.Dashboards;
using LevelUp.API.Services.Interfaces;
using LevelUp.API.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace LevelUp.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet("manager/dashboard")]
    public async Task<IActionResult> GetDashboard([FromQuery] Guid managerId)
    {
        var result = await _dashboardService.GetDashboardAsync(managerId);
        return Ok(new ApiResponse<IEnumerable<DashboardResponse>>(new List<DashboardResponse> { result }));
    }

    [HttpGet("manager/employees")]
    public async Task<IActionResult> GetEmployees([FromQuery] Guid managerId)
    {
        var result = await _dashboardService.GetEmployeesAsync(managerId);
        return Ok(new ApiResponse<IEnumerable<EmployeeListResponse>>(result));
    }

    [HttpGet("manager/employees/{id}/detail")]
    public async Task<IActionResult> GetEmployeeDetail([FromRoute] Guid id, [FromQuery] Guid managerId, CancellationToken cancellationToken)
    {
        var result = await _dashboardService.GetEmployeeDetailAsync(id, managerId, cancellationToken);
        return Ok(new ApiResponse<EmployeeDetailResponse>(result));
    }
    
[HttpPatch("employee/{id}/status")]
public async Task<IActionResult> UpdateEmployeeStatus(
    [FromRoute] Guid id,
    [FromBody] UpdateEmployeeStatusRequest request,
    CancellationToken cancellationToken)
{
    var success = await _dashboardService.UpdateEmployeeStatusAsync(id, request.IsIdle, cancellationToken);
    if (!success) return BadRequest("Failed to update employee status.");
    return Ok(new ApiResponse<string>("Employee status updated successfully."));
}


    [HttpGet("{Id}/enrollments")]
    public async Task<IActionResult> GetEnrollmentsByManager(Guid Id)
    {
        var enrollments = await _dashboardService.GetEnrollmentsByManagerId(Id);
        return Ok(new ApiResponse<IEnumerable<EmployeeEnrollResponse>>(enrollments));
    }
}
