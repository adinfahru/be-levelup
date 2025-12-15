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
        var accountId = User.GetAccountId(); // ✅ JWT source of truth

        var enrollment =
            await _enrollService.GetCurrentEnrollmentAsync(
                accountId,
                cancellationToken);

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
        var accountId = User.GetAccountId();

        var response = await _enrollService.EnrollAsync(
            accountId,
            request.ModuleId,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetCurrentEnrollment),
            new ApiResponse<EnrollmentResponse>(response));
    }

    // =========================
    // SUBMIT CHECKLIST ITEM
    // =========================
    [HttpPost("{enrollmentid:guid}/items")]
    [Authorize(Roles = "Employee")]
    public async Task<IActionResult> SubmitChecklist(
        Guid enrollmentid,
        [FromBody] SubmitChecklistRequest request,
        CancellationToken cancellationToken)
    {
        var accountId = User.GetAccountId();

        var response =
            await _enrollService.SubmitEnrollmentItemAsync(
                enrollmentid,
                accountId,
                request,
                cancellationToken);

        return Ok(new ApiResponse<EnrollmentResponse>(response));
    }

    // =========================
    // GET PROGRESS
    // =========================
    [HttpGet("{enrollmentid:guid}/progress")]
    [Authorize(Roles = "Employee")]
    public async Task<IActionResult> GetProgress(
        Guid enrollmentid,
        CancellationToken cancellationToken)
    {
        var accountId = User.GetAccountId();

        var progress =
            await _enrollService.GetEnrollmentProgressAsync(
                enrollmentid,
                accountId,
                cancellationToken);

        return Ok(new ApiResponse<EnrollmentResponse>(progress));
    }

    // =========================
    // RESUME ENROLLMENT
    // =========================
    [HttpPost("{enrollmentid:guid}/resume")]
    [Authorize(Roles = "Employee")]
    public async Task<IActionResult> Resume(
        Guid enrollmentid,
        CancellationToken cancellationToken)
    {
        var accountId = User.GetAccountId();

        var response =
            await _enrollService.ResumeEnrollmentAsync(
                enrollmentid,
                accountId,
                cancellationToken);

        return Ok(new ApiResponse<EnrollmentResponse>(response));
    }

    // =========================
    // GET HISTORY
    // =========================
    [HttpGet("history")]
    [Authorize(Roles = "Employee")]
    public async Task<IActionResult> GetHistory(
        CancellationToken cancellationToken)
    {
        var accountId = User.GetAccountId();

        var history =
            await _enrollService.GetEnrollmentHistoryAsync(
                accountId,
                cancellationToken);

        return Ok(new ApiResponse<List<EnrollmentResponse>>(history));
    }
}