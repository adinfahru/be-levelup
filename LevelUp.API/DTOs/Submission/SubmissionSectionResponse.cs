namespace LevelUp.API.DTOs.Submissions;

public record SubmissionSectionResponse(
    int OrderIndex,
    string Title,
    string? Description,
    string? EvidenceUrl,
    string Status // Completed | OnProgress | Locked
);
