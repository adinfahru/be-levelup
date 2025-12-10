using System;
using System.Collections.Generic;

namespace LevelUp.API.Entity
{
    public class Employee
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Guid? PositionId { get; set; }
        public bool IsIdle { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public Account Account { get; set; }
        public Employee Manager { get; set; }
        public Position Position { get; set; }
        public ICollection<Module> CreatedModules { get; set; } = new List<Module>();
    }
}
