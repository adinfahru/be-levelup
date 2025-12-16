using LevelUp.API.Entity;

public record SubmissionListResponse(
    Guid SubmissionId,
    Guid EnrollmentId,

    Guid EmployeeId,
    string EmployeeName,
    string Email,

    Guid ModuleId,
    string ModuleTitle,

    int CompletedCount,
    int TotalCount,

    string Status,
    DateTime SubmittedAt
);
