using LevelUp.API.Entity;

public record SubmissionReviewResponse(
    Guid SubmissionId,
    SubmissionStatus Status,
    string? ManagerFeedback,
    DateTime ReviewedAt
);
