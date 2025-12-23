using LevelUp.API.DTOs.Enrolls;
using LevelUp.API.Services.Interfaces;
using LevelUp.API.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LevelUp.API.Controllers;

[ApiController]
[Route("api/v1/enrollments")]
[Authorize]
public class EnrollmentController : ControllerBase
{
    private readonly IEnrollmentService _enrollmentService;

    public EnrollmentController(IEnrollmentService enrollmentService)
    {
        _enrollmentService = enrollmentService;
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
            await _enrollmentService.GetCurrentEnrollmentAsync(
                accountId,
                cancellationToken);

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
        var accountId = User.GetAccountId();

        var response = await _enrollmentService.EnrollAsync(
            accountId,
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
        var accountId = User.GetAccountId();

        var response =
            await _enrollmentService.SubmitEnrollmentItemAsync(
                enrollmentid,
                accountId,
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
        var accountId = User.GetAccountId();

        var progress =
            await _enrollmentService.GetEnrollmentProgressAsync(
                enrollmentid,
                accountId,
                cancellationToken);

        return Ok(new ApiResponse<EnrollmentResponse>(progress));
    }

    [HttpPost("{enrollmentid:guid}/resume")]
    [Authorize(Roles = "Employee")]
    public async Task<IActionResult> Resume(
        Guid enrollmentid,
        CancellationToken cancellationToken)
    {
        var accountId = User.GetAccountId();

        var response =
            await _enrollmentService.ResumeEnrollmentAsync(
                enrollmentid,
                accountId,
                cancellationToken);

        return Ok(new ApiResponse<EnrollmentResponse>(response));
    }

    [HttpGet("history")]
    [Authorize(Roles = "Employee")]
    public async Task<IActionResult> GetHistory(
        CancellationToken cancellationToken)
    {
        var accountId = User.GetAccountId();

        var history =
            await _enrollmentService.GetEnrollmentHistoryAsync(
                accountId,
                cancellationToken);

        return Ok(new ApiResponse<List<EnrollmentResponse>>(history));
    }

    [HttpPost("assign")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> AssignEnrollment(
    [FromBody] AssignEnrollmentRequest request,
    CancellationToken cancellationToken
)
    {
        var managerId = User.GetAccountId();

        var result = await _enrollmentService.AssignEnrollmentAsync(
            managerId,
            request,
            cancellationToken
        );

        return Ok(new ApiResponse<EnrollmentResponse>(
            200,
            "Enrollment assigned successfully",
            result
        ));
    }
}