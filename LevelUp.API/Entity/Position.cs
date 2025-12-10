using System;
using System.Collections.Generic;

namespace LevelUp.API.Entity
{
    public class Position
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public bool IsActive { get; set; }

        // Navigation properties
        public ICollection<Employee>? Employees { get; set; } = new List<Employee>();
    }
}
