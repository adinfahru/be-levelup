using LevelUp.API.DTOs.Dashboards;
using LevelUp.API.Services.Interfaces;
using LevelUp.API.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LevelUp.API.Controllers;

[ApiController]
[Route("[controller]")]
// [Authorize(Roles = "Admin,Manager")]
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

    [HttpGet("manager/employees/{Id}/detail")]
    public async Task<IActionResult> GetEmployeeDetail([FromRoute] Guid Id, [FromQuery] Guid managerId)
    {
        var result = await _dashboardService.GetEmployeeDetailAsync(Id, managerId);
        return Ok(new ApiResponse<EmployeeDetailResponse>(result));
    }

    [HttpPatch("employee/{Id}/status")]
    // [Authorize(Roles = "Manager")]
    public async Task<IActionResult> UpdateEmployeeStatus([FromRoute] Guid Id, [FromQuery] bool isIdle)
    {
        var success = await _dashboardService.UpdateEmployeeStatusAsync(Id, isIdle);
        if (!success)
        {
            return BadRequest("Failed to update employee status.");
        }
        return Ok(new ApiResponse<string>("Employee status updated successfully."));
    }
}
