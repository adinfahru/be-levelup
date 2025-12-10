using System;
using System.Collections.Generic;

namespace LevelUp.API.Entity
{
    public class Enrollment
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public Guid ModuleId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime TargetDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public EnrollmentStatus Status { get; set; }
        public int CurrentProgress { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public Account? Account { get; set; }
        public Module? Module { get; set; }
        public ICollection<EnrollmentItem> Items { get; set; } = new List<EnrollmentItem>();
        public ICollection<Submission> Submissions { get; set; } = new List<Submission>();
    }

    public enum EnrollmentStatus
    {
        OnGoing = 0,
        Paused = 1,
        Completed = 2,
    }
}
