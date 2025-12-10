using System;

namespace LevelUp.API.Entity
{
    public class EnrollmentItem
    {
        public Guid Id { get; set; }
        public Guid EnrollmentId { get; set; }
        public Guid ModuleItemId { get; set; }
        public bool IsCompleted { get; set; }
        public string Feedback { get; set; }
        public string EvidenceUrl { get; set; }
        public DateTime? CompletedAt { get; set; }

        // Navigation properties
        public Enrollment Enrollment { get; set; }
        public ModuleItem ModuleItem { get; set; }
    }
}
