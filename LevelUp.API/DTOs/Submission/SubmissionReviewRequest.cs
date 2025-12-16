using LevelUp.API.Entity;

public class SubmissionReviewRequest
{
    public SubmissionStatus Status { get; set; }
    public string? ManagerFeedback { get; set; }
}
