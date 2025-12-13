namespace LevelUp.API.DTOs.Enrolls;

public record SubmitChecklistRequest
(
    Guid ModuleItemId,
    string? EvidenceUrl,
    string? Feedback
);
