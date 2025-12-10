using System;
using System.Collections.Generic;

namespace LevelUp.API.Entity
{
    public class ModuleItem
    {
        public Guid Id { get; set; }
        public Guid ModuleId { get; set; }
        public string Title { get; set; }
        public int OrderIndex { get; set; }
        public string Descriptions { get; set; }
        public string Url { get; set; }
        public bool IsFinalSubmission { get; set; }

        // Navigation properties
        public Module Module { get; set; }
        public ICollection<EnrollmentItem> EnrollmentItems { get; set; } =
            new List<EnrollmentItem>();
    }
}
