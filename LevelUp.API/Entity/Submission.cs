using System;

namespace LevelUp.API.Entity
{
    public class Submission
    {
        public Guid Id { get; set; }
        public Guid EnrollmentId { get; set; }
        public SubmissionStatus Status { get; set; }
        public string? Notes { get; set;}
        public string? ManagerFeedback { get; set; }
        public int? EstimatedDays { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public Enrollment? Enrollment { get; set; }
    }

    public enum SubmissionStatus
    {
        Pending,
        Approved,
        Rejected,
    }
}
