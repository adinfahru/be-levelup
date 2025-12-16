using System.ComponentModel.DataAnnotations;
using LevelUp.API.Entity;

namespace LevelUp.API.DTOs.Submissions;

public class SubmissionReviewRequest
{
    [Required]
    public string? Status { get; set; }

    public string? ManagerFeedback { get; set; }

    public int? EstimatedDays { get; set; } // ✅ int
}
