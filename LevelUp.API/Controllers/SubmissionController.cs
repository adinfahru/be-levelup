using LevelUp.API.DTOs;
using LevelUp.API.DTOs.Submissions;
using LevelUp.API.Services.Interfaces;
using LevelUp.API.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LevelUp.API.Controllers;

[ApiController]
[Route("api/v1/submissions")]
[Authorize(Roles = "Manager")]
public class SubmissionController : ControllerBase
{
    private readonly ISubmissionService _submissionService;

    public SubmissionController(ISubmissionService submissionService)
    {
        _submissionService = submissionService;
    }

    // ===============================
    // GET LIST SUBMISSIONS
    // ===============================
    [HttpGet]
    public async Task<IActionResult> GetSubmissions()
    {
        var result = await _submissionService.GetSubmissionsAsync();

        return Ok(new ApiResponse<IEnumerable<SubmissionListResponse>>(
            result
        ));
    }

    // ===============================
    // GET SUBMISSION DETAIL
    // ===============================
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetSubmissionDetail(Guid id)
    {
        var result = await _submissionService.GetSubmissionDetailAsync(id);

        if (result == null)
            return NotFound(new ApiResponse<string>("Submission not found"));

        return Ok(new ApiResponse<SubmissionDetailResponse>(result));
    }

    // ===============================
    // REVIEW SUBMISSION
    // ===============================
    [HttpPatch("{id:guid}/review")]
    public async Task<IActionResult> ReviewSubmission(
        Guid id,
        [FromBody] SubmissionReviewRequest request
    )
    {
        if (!ModelState.IsValid)
            return BadRequest(new ApiResponse<string>("Invalid request"));

        var result = await _submissionService.ReviewSubmissionAsync(id, request);

        return Ok(new ApiResponse<SubmissionReviewResponse>(
            200,
            "Submission reviewed successfully",
            result
));

    }
}
