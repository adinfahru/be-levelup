using LevelUp.API.DTOs.Enrolls;
using LevelUp.API.Services.Interfaces;
using LevelUp.API.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LevelUp.API.Controllers;

[ApiController]
[Route("api/v1/enrollments")]
[Authorize]
public class EnrollController : ControllerBase
{
    private readonly IEnrollService _enrollService;

    public EnrollController(IEnrollService enrollService)
    {
        _enrollService = enrollService;
    }

    // =========================
    // GET CURRENT ENROLLMENT
    // =========================
    [HttpGet("current")]
    [Authorize(Roles = "Employee")]
    public async Task<IActionResult> GetCurrentEnrollment(
        CancellationToken cancellationToken)
    {
        var email = User.GetEmail();

        var enrollment =
            await _enrollService.GetCurrentEnrollmentAsync(
                email, cancellationToken);

        if (enrollment is null)
            return NoContent(); // 204

        return Ok(new ApiResponse<EnrollmentResponse>(enrollment));
    }

    // =========================
    // ENROLL MODULE
    // =========================
    [HttpPost]
    [Authorize(Roles = "Employee")]
    public async Task<IActionResult> Enroll(
        [FromBody] EnrollmentRequest request,
        CancellationToken cancellationToken)
    {
        var email = User.GetEmail();

        var response = await _enrollService.EnrollAsync(
            email,
            request.ModuleId,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetCurrentEnrollment),
            new ApiResponse<EnrollmentResponse>(response));
    }

    // =========================
    // RESUME ENROLLMENT
    // =========================
    [HttpPost("{id:guid}/resume")]
    [Authorize(Roles = "Employee")]
    public async Task<IActionResult> Resume(
        Guid id,
        CancellationToken cancellationToken)
    {
        var email = User.GetEmail();

        var response =
            await _enrollService.ResumeEnrollmentAsync(
                id,
                email,
                cancellationToken);

        return Ok(new ApiResponse<EnrollmentResponse>(response));
    }

    // =========================
    // ENROLLMENT HISTORY
    // =========================
    [HttpGet("history")]
    [Authorize(Roles = "Employee")]
    public async Task<IActionResult> GetHistory(
        CancellationToken cancellationToken)
    {
        var email = User.GetEmail();

        var history =
            await _enrollService.GetEnrollmentHistoryAsync(
                email,
                cancellationToken);

        return Ok(new ApiResponse<List<EnrollmentResponse>>(history));
    }
}