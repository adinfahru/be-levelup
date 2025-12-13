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

            await context.Accounts.AddRangeAsync(adminAccount, managerAccount, employeeAccount);
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

            await context.Employees.AddRangeAsync(adminEmployee, managerEmployee, employeeEmployee);
            await context.SaveChangesAsync();

            // Seed Modules
            var module1 = new Module
            {
                Id = Guid.NewGuid(),
                Title = "Introduction to ASP.NET Core",
                Description = "Learn the fundamentals of ASP.NET Core web development",
                EstimatedDays = 7,
                IsActive = true,
                CreatedBy = managerAccount.Id,
                CreatedAt = DateTime.UtcNow.AddDays(-10),
            };

            var module2 = new Module
            {
                Id = Guid.NewGuid(),
                Title = "Advanced C# Programming",
                Description = "Master advanced C# concepts including async/await, LINQ, and delegates",
                EstimatedDays = 14,
                IsActive = true,
                CreatedBy = managerAccount.Id,
                CreatedAt = DateTime.UtcNow.AddDays(-8),
            };

            var module3 = new Module
            {
                Id = Guid.NewGuid(),
                Title = "Microservices Architecture",
                Description = "Building scalable microservices with .NET and Docker",
                EstimatedDays = 21,
                IsActive = false,
                CreatedBy = managerAccount.Id,
                CreatedAt = DateTime.UtcNow.AddDays(-5),
            };

            var module4 = new Module
            {
                Id = Guid.NewGuid(),
                Title = "Database Design with Entity Framework",
                Description = "Learn to design and implement databases using EF Core",
                EstimatedDays = 10,
                IsActive = true,
                CreatedBy = managerAccount.Id,
                CreatedAt = DateTime.UtcNow.AddDays(-3),
            };

            var module5 = new Module
            {
                Id = Guid.NewGuid(),
                Title = "React Fundamentals",
                Description = "Build modern web applications with React and TypeScript",
                EstimatedDays = 12,
                IsActive = true,
                CreatedBy = managerAccount.Id,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
            };

            await context.Modules.AddRangeAsync(module1, module2, module3, module4, module5);
            await context.SaveChangesAsync();

            // Seed Module Items for Module 1 (ASP.NET Core)
            var module1Items = new List<ModuleItem>
            {
                new ModuleItem
                {
                    Id = Guid.NewGuid(),
                    ModuleId = module1.Id,
                    Title = "Setup Development Environment",
                    OrderIndex = 1,
                    Descriptions = "Install Visual Studio and .NET SDK",
                    Url = "https://docs.microsoft.com/aspnet/core",
                    IsFinalSubmission = false,
                },
                new ModuleItem
                {
                    Id = Guid.NewGuid(),
                    ModuleId = module1.Id,
                    Title = "Create First API",
                    OrderIndex = 2,
                    Descriptions = "Build a simple REST API",
                    Url = "https://docs.microsoft.com/aspnet/core/tutorials",
                    IsFinalSubmission = false,
                },
                new ModuleItem
                {
                    Id = Guid.NewGuid(),
                    ModuleId = module1.Id,
                    Title = "Final Project Submission",
                    OrderIndex = 3,
                    Descriptions = "Submit your completed API project",
                    Url = "https://github.com",
                    IsFinalSubmission = true,
                },
            };

            // Seed Module Items for Module 2 (Advanced C#)
            var module2Items = new List<ModuleItem>
            {
                new ModuleItem
                {
                    Id = Guid.NewGuid(),
                    ModuleId = module2.Id,
                    Title = "Async/Await Deep Dive",
                    OrderIndex = 1,
                    Descriptions = "Understanding async programming patterns",
                    Url = "https://docs.microsoft.com/dotnet/csharp/async",
                    IsFinalSubmission = false,
                },
                new ModuleItem
                {
                    Id = Guid.NewGuid(),
                    ModuleId = module2.Id,
                    Title = "LINQ Mastery",
                    OrderIndex = 2,
                    Descriptions = "Master LINQ queries and operators",
                    Url = "https://docs.microsoft.com/dotnet/csharp/linq",
                    IsFinalSubmission = false,
                },
                new ModuleItem
                {
                    Id = Guid.NewGuid(),
                    ModuleId = module2.Id,
                    Title = "Delegates and Events",
                    OrderIndex = 3,
                    Descriptions = "Learn delegates, events, and callbacks",
                    Url = "https://docs.microsoft.com/dotnet/csharp/delegates",
                    IsFinalSubmission = false,
                },
                new ModuleItem
                {
                    Id = Guid.NewGuid(),
                    ModuleId = module2.Id,
                    Title = "Generics and Collections",
                    OrderIndex = 4,
                    Descriptions = "Deep dive into generic types and collections",
                    Url = "https://docs.microsoft.com/dotnet/csharp/generics",
                    IsFinalSubmission = false,
                },
                new ModuleItem
                {
                    Id = Guid.NewGuid(),
                    ModuleId = module2.Id,
                    Title = "Final Project",
                    OrderIndex = 5,
                    Descriptions = "Build a complete async application",
                    Url = "https://github.com",
                    IsFinalSubmission = true,
                },
            };

            // Seed Module Items for Module 3 (Microservices)
            var module3Items = new List<ModuleItem>
            {
                new ModuleItem
                {
                    Id = Guid.NewGuid(),
                    ModuleId = module3.Id,
                    Title = "Microservices Introduction",
                    OrderIndex = 1,
                    Descriptions = "Core concepts and patterns",
                    Url = "https://microservices.io",
                    IsFinalSubmission = false,
                },
                new ModuleItem
                {
                    Id = Guid.NewGuid(),
                    ModuleId = module3.Id,
                    Title = "Docker Containerization",
                    OrderIndex = 2,
                    Descriptions = "Containerize your services",
                    Url = "https://docker.com/docs",
                    IsFinalSubmission = false,
                },
                new ModuleItem
                {
                    Id = Guid.NewGuid(),
                    ModuleId = module3.Id,
                    Title = "Final Microservices Project",
                    OrderIndex = 3,
                    Descriptions = "Build complete microservices system",
                    Url = "https://github.com",
                    IsFinalSubmission = true,
                },
            };

            // Seed Module Items for Module 4 (EF Core) - No items yet
            var module4Items = new List<ModuleItem>
            {
                new ModuleItem
                {
                    Id = Guid.NewGuid(),
                    ModuleId = module4.Id,
                    Title = "EF Core Basics",
                    OrderIndex = 1,
                    Descriptions = "Learn the fundamentals of Entity Framework Core",
                    Url = "https://docs.microsoft.com/ef/core",
                    IsFinalSubmission = false,
                },
                new ModuleItem
                {
                    Id = Guid.NewGuid(),
                    ModuleId = module4.Id,
                    Title = "Database Migrations",
                    OrderIndex = 2,
                    Descriptions = "Managing database schema changes",
                    Url = "https://docs.microsoft.com/ef/core/migrations",
                    IsFinalSubmission = false,
                },
                new ModuleItem
                {
                    Id = Guid.NewGuid(),
                    ModuleId = module4.Id,
                    Title = "Final Database Project",
                    OrderIndex = 3,
                    Descriptions = "Design and implement complete database",
                    Url = "https://github.com",
                    IsFinalSubmission = true,
                },
            };

            // Module 5 has no items (empty module for testing)

            await context.ModuleItems.AddRangeAsync(module1Items);
            await context.ModuleItems.AddRangeAsync(module2Items);
            await context.ModuleItems.AddRangeAsync(module3Items);
            await context.ModuleItems.AddRangeAsync(module4Items);
            await context.SaveChangesAsync();

            var moduleItems = module1Items; // Keep reference for enrollment seeding

            // Seed Enrollment
            var enrollment = new Enrollment
            {
                Id = Guid.NewGuid(),
                AccountId = employeeAccount.Id,
                ModuleId = module1.Id,
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
        }
    }
}