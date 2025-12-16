namespace LevelUp.API.DTOs.Submissions;

public record SubmissionReviewRequest(
    string Status,              // Approved | Rejected
    string? ManagerFeedback,    // wajib kalau Rejected
    int? TargetDays             // jumlah hari untuk revisi
);
