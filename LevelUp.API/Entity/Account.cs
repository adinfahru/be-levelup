using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LevelUp.API.Entity
{
    public class Account
    {
        public Guid Id { get; set; }
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }
        public UserRole Role { get; set; }
        public int? Otp { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public Employee? Employee { get; set; }
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public ICollection<Module> CreatedModules { get; set; } = new List<Module>();
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum UserRole
    {
        Admin,
        Manager,
        Employee,
    }
}
