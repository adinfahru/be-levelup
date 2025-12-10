using System;
using System.Collections.Generic;

namespace LevelUp.API.Entity
{
    public class Module
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int EstimatedDays { get; set; }
        public bool IsActive { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        // Navigation properties
        public Employee Creator { get; set; }
        public ICollection<ModuleItem> Items { get; set; } = new List<ModuleItem>();
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}
