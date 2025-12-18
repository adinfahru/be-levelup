using LevelUp.API.Entity;

public record SubmissionReviewResponse(
    Guid SubmissionId,
    string Status,
    string? ManagerFeedback,
    int? EstimatedDays
);
