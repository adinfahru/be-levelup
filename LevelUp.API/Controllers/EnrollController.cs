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

    [HttpPost("{enrollmentid:guid}/items")]
    [Authorize(Roles = "Employee")]
    public async Task<IActionResult> SubmitChecklist(
        Guid enrollmentid,
        [FromBody] SubmitChecklistRequest request,
        CancellationToken cancellationToken)
    {
        var email = User.GetEmail();

        var response =
            await _enrollService.SubmitEnrollmentItemAsync(
                enrollmentid,
                email,
                request,
                cancellationToken);

        return Ok(new ApiResponse<EnrollmentResponse>(response));
    }

    [HttpGet("{enrollmentid:guid}/progress")]
    [Authorize(Roles = "Employee")]
    public async Task<IActionResult> GetProgress(
        Guid enrollmentid,
        CancellationToken cancellationToken)
    {
        var email = User.GetEmail();

        var progress =
            await _enrollService.GetEnrollmentProgressAsync(
                enrollmentid,
                email,
                cancellationToken);

        return Ok(new ApiResponse<EnrollmentResponse>(progress));
    }

    [HttpPost("{enrollmentid:guid}/resume")]
    [Authorize(Roles = "Employee")]
    public async Task<IActionResult> Resume(
        Guid enrollmentid,
        CancellationToken cancellationToken)
    {
        var email = User.GetEmail();

        var response =
            await _enrollService.ResumeEnrollmentAsync(
                enrollmentid,
                email,
                cancellationToken);

        return Ok(new ApiResponse<EnrollmentResponse>(response));
    }

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