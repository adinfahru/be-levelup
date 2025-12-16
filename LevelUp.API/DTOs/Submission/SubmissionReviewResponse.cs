namespace LevelUp.API.DTOs.Submissions;

public record SubmissionReviewResponse(
    Guid SubmissionId,
    string Status,
    string? ManagerFeedback,
    DateTime ReviewedAt
);
