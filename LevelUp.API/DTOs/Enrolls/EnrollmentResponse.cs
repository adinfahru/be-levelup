using LevelUp.API.Entity;

namespace LevelUp.API.DTOs.Enrolls;

public record EnrollmentResponse
(
    Guid EnrollmentId,
    Guid ModuleId,
    string ModuleTitle,
    string ModuleDescription,
    DateTime StartDate,
    DateTime TargetDate,
    DateTime? CompletedDate,
    EnrollmentStatus Status,
    int CurrentProgress,
    List<EnrollmentItemDto> Sections
);

public record EnrollmentItemDto
(
    Guid EnrollmentItemId,
    Guid ModuleItemId,
    int OrderIndex,
    string ModuleItemTitle,
    string ModuleItemDescription,
    string ModuleItemUrl,
    bool IsFinalSubmission,
    bool IsCompleted,
    string? EvidenceUrl,
    DateTime? CompletedAt
);
