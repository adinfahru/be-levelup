using LevelUp.API.Entity;

namespace LevelUp.API.Data
{
    public class LevelUpDataSeeder
    {
        public static async Task SeedAsync(LevelUpDbContext context)
        {
            // Check if data already exists
            if (context.Accounts.Any())
            {
                return; // Database already seeded
            }

            // Seed Positions
            var positions = new List<Position>
            {
                new Position
                {
                    Id = Guid.NewGuid(),
                    Title = "Fullstack .NET Developer",
                    IsActive = true,
                },
                new Position
                {
                    Id = Guid.NewGuid(),
                    Title = "Fullstack Java Developer",
                    IsActive = true,
                },
                new Position
                {
                    Id = Guid.NewGuid(),
                    Title = "Quality Assurance",
                    IsActive = true,
                },
                new Position
                {
                    Id = Guid.NewGuid(),
                    Title = "Data Minning",
                    IsActive = true,
                },
            };
            await context.Positions.AddRangeAsync(positions);
            await context.SaveChangesAsync();

            // Seed Accounts
            var adminAccount = new Account
            {
                Id = Guid.NewGuid(),
                Email = "admin@levelup.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                Role = UserRole.Admin,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
            };

            var managerAccount = new Account
            {
                Id = Guid.NewGuid(),
                Email = "manager@levelup.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Manager123!"),
                Role = UserRole.Manager,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
            };

            var employeeAccount = new Account
            {
                Id = Guid.NewGuid(),
                Email = "employee@levelup.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Employee123!"),
                Role = UserRole.Employee,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
            };

            var employeeAccount2 = new Account
            {
                Id = Guid.NewGuid(),
                Email = "employee2@levelup.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Employee123!"),
                Role = UserRole.Employee,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
            };

            var employeeAccount3 = new Account
            {
                Id = Guid.NewGuid(),
                Email = "employee3@levelup.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Employee123!"),
                Role = UserRole.Employee,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
            };

            var employeeAccount4 = new Account
            {
                Id = Guid.NewGuid(),
                Email = "employee4@levelup.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Employee123!"),
                Role = UserRole.Employee,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
            };

            var employeeAccount5 = new Account
            {
                Id = Guid.NewGuid(),
                Email = "employee5@levelup.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Employee123!"),
                Role = UserRole.Employee,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
            };

            await context.Accounts.AddRangeAsync(adminAccount, managerAccount, employeeAccount, employeeAccount2, employeeAccount3, employeeAccount4, employeeAccount5);
            await context.SaveChangesAsync();

            // Seed Employees
            var adminEmployee = new Employee
            {
                Id = Guid.NewGuid(),
                AccountId = adminAccount.Id,
                FirstName = "Admin",
                LastName = "User",
                PositionId = positions[2].Id, // Tech Lead
                IsIdle = false,
                CreatedAt = DateTime.UtcNow,
            };

            var managerEmployee = new Employee
            {
                Id = Guid.NewGuid(),
                AccountId = managerAccount.Id,
                FirstName = "Manager",
                LastName = "User",
                PositionId = positions[1].Id, // Senior Developer
                IsIdle = false,
                CreatedAt = DateTime.UtcNow,
            };

            var employeeEmployee = new Employee
            {
                Id = Guid.NewGuid(),
                AccountId = employeeAccount.Id,
                FirstName = "John",
                LastName = "Doe",
                PositionId = positions[0].Id, // Junior Developer
                IsIdle = true,
                CreatedAt = DateTime.UtcNow,
            };

            // EXTRA EMPLOYEES (4 orang)
            var employee2 = new Employee
            {
                Id = Guid.NewGuid(),
                AccountId = employeeAccount2.Id,
                FirstName = "Sarah",
                LastName = "Wati",
                PositionId = positions[0].Id,
                IsIdle = false, // ACTIVE
                CreatedAt = DateTime.UtcNow,
            };

            var employee3 = new Employee
            {
                Id = Guid.NewGuid(),
                AccountId = employeeAccount3.Id,
                FirstName = "Dimas",
                LastName = "Santoso",
                PositionId = positions[1].Id,
                IsIdle = false, // ACTIVE
                CreatedAt = DateTime.UtcNow,
            };

            var employee4 = new Employee
            {
                Id = Guid.NewGuid(),
                AccountId = employeeAccount4.Id,
                FirstName = "Rina",
                LastName = "Amelia",
                PositionId = positions[2].Id,
                IsIdle = true, // IDLE
                CreatedAt = DateTime.UtcNow,
            };

            var employee5 = new Employee
            {
                Id = Guid.NewGuid(),
                AccountId = employeeAccount5.Id,
                FirstName = "Andi",
                LastName = "Saputra",
                PositionId = positions[1].Id,
                IsIdle = true, // IDLE
                CreatedAt = DateTime.UtcNow,
            };

            await context.Employees.AddRangeAsync(adminEmployee, managerEmployee, employeeEmployee,
                employee2, employee3, employee4, employee5);
            await context.SaveChangesAsync();

            // Seed Module
            var module = new Module
            {
                Id = Guid.NewGuid(),
                Title = "Introduction to ASP.NET Core",
                Description = "Learn the fundamentals of ASP.NET Core web development",
                EstimatedDays = 7,
                IsActive = true,
                CreatedBy = managerAccount.Id,
                CreatedAt = DateTime.UtcNow,
            };
            await context.Modules.AddAsync(module);
            await context.SaveChangesAsync();

            // Seed Module Items
            var moduleItems = new List<ModuleItem>
            {
                new ModuleItem
                {
                    Id = Guid.NewGuid(),
                    ModuleId = module.Id,
                    Title = "Setup Development Environment",
                    OrderIndex = 1,
                    Descriptions = "Install Visual Studio and .NET SDK",
                    Url = "https://docs.microsoft.com/aspnet/core",
                    IsFinalSubmission = false,
                },
                new ModuleItem
                {
                    Id = Guid.NewGuid(),
                    ModuleId = module.Id,
                    Title = "Create First API",
                    OrderIndex = 2,
                    Descriptions = "Build a simple REST API",
                    Url = "https://docs.microsoft.com/aspnet/core/tutorials",
                    IsFinalSubmission = false,
                },
                new ModuleItem
                {
                    Id = Guid.NewGuid(),
                    ModuleId = module.Id,
                    Title = "Final Project Submission",
                    OrderIndex = 3,
                    Descriptions = "Submit your completed API project",
                    Url = "https://github.com",
                    IsFinalSubmission = true,
                },
            };
            await context.ModuleItems.AddRangeAsync(moduleItems);
            await context.SaveChangesAsync();

            // Seed Enrollment
            var enrollment = new Enrollment
            {
                Id = Guid.NewGuid(),
                AccountId = employeeAccount.Id,
                ModuleId = module.Id,
                StartDate = DateTime.UtcNow.AddDays(-3),
                TargetDate = DateTime.UtcNow.AddDays(4),
                Status = EnrollmentStatus.OnGoing,
                CurrentProgress = 33,
                CreatedAt = DateTime.UtcNow,
            };
            await context.Enrollments.AddAsync(enrollment);
            await context.SaveChangesAsync();

            // Seed Enrollment Items
            var enrollmentItems = new List<EnrollmentItem>
            {
                new EnrollmentItem
                {
                    Id = Guid.NewGuid(),
                    EnrollmentId = enrollment.Id,
                    ModuleItemId = moduleItems[0].Id,
                    IsCompleted = true,
                    EvidenceUrl = "https://github.com/user/repo/commit/123",
                    CompletedAt = DateTime.UtcNow.AddDays(-2),
                },
                new EnrollmentItem
                {
                    Id = Guid.NewGuid(),
                    EnrollmentId = enrollment.Id,
                    ModuleItemId = moduleItems[1].Id,
                    IsCompleted = false,
                },
                new EnrollmentItem
                {
                    Id = Guid.NewGuid(),
                    EnrollmentId = enrollment.Id,
                    ModuleItemId = moduleItems[2].Id,
                    IsCompleted = false,
                },
            };
            await context.EnrollmentItems.AddRangeAsync(enrollmentItems);
            await context.SaveChangesAsync();


var submissionPending = new Submission
{
    Id = Guid.NewGuid(),
    EnrollmentId = enrollment.Id,
    Status = SubmissionStatus.Pending,
    Notes = "Sudah menyelesaikan setup environment dan mulai membuat API dasar.",
    ManagerFeedback = null,
    CreatedAt = DateTime.UtcNow.AddDays(-1),
};

var submissionRejected = new Submission
{
    Id = Guid.NewGuid(),
    EnrollmentId = enrollment.Id,
    Status = SubmissionStatus.Rejected,
    Notes = "CRUD sudah berjalan, namun validasi dan error handling masih kurang.",
    ManagerFeedback = "Perbaiki validasi input dan tambahkan global exception handling. Deadline 3 hari.",
    CreatedAt = DateTime.UtcNow.AddDays(-2),
};

await context.Submissions.AddRangeAsync(
    submissionPending,
    submissionRejected
);

await context.SaveChangesAsync();

        }
    }
}