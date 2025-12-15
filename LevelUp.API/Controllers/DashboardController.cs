using System.Security.Claims;
using LevelUp.API.DTOs.Dashboards;
using LevelUp.API.Services.Interfaces;
using LevelUp.API.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LevelUp.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Roles = "Manager")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    // DASHBOARD STATS
    [HttpGet("manager/dashboard")]
    public async Task<IActionResult> GetDashboard()
    {
        var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(accountIdClaim, out var accountId))
            return Unauthorized("Invalid account id in token");

        var result = await _dashboardService.GetDashboardAsync(accountId);

        return Ok(new ApiResponse<IEnumerable<DashboardResponse>>(
            new List<DashboardResponse> { result }
        ));
    }

    // EMPLOYEES
    [HttpGet("manager/employees")]
    public async Task<IActionResult> GetEmployees()
    {
        var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(accountIdClaim, out var accountId))
            return Unauthorized("Invalid account id in token");

        var result = await _dashboardService.GetEmployeesAsync(accountId);
        return Ok(new ApiResponse<IEnumerable<EmployeeListResponse>>(result));
    }

    // EMPLOYEE DETAIL
    [HttpGet("manager/employees/{id}/detail")]
    public async Task<IActionResult> GetEmployeeDetail(
        Guid id,
        CancellationToken cancellationToken)
    {
        var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(accountIdClaim, out var accountId))
            return Unauthorized("Invalid account id in token");

        var result = await _dashboardService.GetEmployeeDetailAsync(
            id,
            accountId,
            cancellationToken
        );

        return Ok(new ApiResponse<EmployeeDetailResponse>(result));
    }

    // UPDATE STATUS
    [HttpPatch("employee/{id}/status")]
    public async Task<IActionResult> UpdateEmployeeStatus(
        Guid id,
        [FromBody] UpdateEmployeeStatusRequest request,
        CancellationToken cancellationToken)
    {
        var success = await _dashboardService
            .UpdateEmployeeStatusAsync(id, request.IsIdle, cancellationToken);

        if (!success)
            return BadRequest("Failed to update employee status");

        return Ok(new ApiResponse<string>("Employee status updated"));
    }

    // ENROLLMENTS
    [HttpGet("manager/enrollments")]
    public async Task<IActionResult> GetEnrollments()
    {
        var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(accountIdClaim, out var accountId))
            return Unauthorized("Invalid account id in token");

        var enrollments =
            await _dashboardService.GetEnrollmentsByManagerId(accountId);

        return Ok(new ApiResponse<IEnumerable<EmployeeEnrollResponse>>(enrollments));
    }
}
