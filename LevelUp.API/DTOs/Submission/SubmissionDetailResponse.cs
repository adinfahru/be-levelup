using LevelUp.API.DTOs.Submissions;
using LevelUp.API.Entity;

public record SubmissionDetailResponse(
    Guid SubmissionId,
    string Status,

    string EmployeeName,
    string Email,

    string ModuleTitle,

    string? Notes,
    string? ManagerFeedback,

    int CompletedCount,
    int TotalCount,

    IEnumerable<SubmissionSectionResponse> Sections
);
