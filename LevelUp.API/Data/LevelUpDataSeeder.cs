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

            var employee2Account = new Account
            {
                Id = Guid.NewGuid(),
                Email = "employee2@levelup.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Employee123!"),
                Role = UserRole.Employee,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
            };

            var employee3Account = new Account
            {
                Id = Guid.NewGuid(),
                Email = "employee3@levelup.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Employee123!"),
                Role = UserRole.Employee,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
            };

            var employee4Account = new Account
            {
                Id = Guid.NewGuid(),
                Email = "employee4@levelup.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Employee123!"),
                Role = UserRole.Employee,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
            };

            var employee5Account = new Account
            {
                Id = Guid.NewGuid(),
                Email = "employee5@levelup.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Employee123!"),
                Role = UserRole.Employee,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
            };

            await context.Accounts.AddRangeAsync(adminAccount, managerAccount, employeeAccount,
                employee2Account, employee3Account, employee4Account, employee5Account);
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

            var employee2 = new Employee
            {
                Id = Guid.NewGuid(),
                AccountId = employee2Account.Id,
                FirstName = "Jessica",
                LastName = "Martinez",
                PositionId = positions[0].Id,
                IsIdle = false,
                CreatedAt = DateTime.UtcNow,
            };

            var employee3 = new Employee
            {
                Id = Guid.NewGuid(),
                AccountId = employee3Account.Id,
                FirstName = "Christopher",
                LastName = "Taylor",
                PositionId = positions[1].Id,
                IsIdle = false,
                CreatedAt = DateTime.UtcNow,
            };

            var employee4 = new Employee
            {
                Id = Guid.NewGuid(),
                AccountId = employee4Account.Id,
                FirstName = "Amanda",
                LastName = "Davis",
                PositionId = positions[0].Id,
                IsIdle = true,
                CreatedAt = DateTime.UtcNow,
            };

            var employee5 = new Employee
            {
                Id = Guid.NewGuid(),
                AccountId = employee5Account.Id,
                FirstName = "Daniel",
                LastName = "Thompson",
                PositionId = positions[2].Id,
                IsIdle = false,
                CreatedAt = DateTime.UtcNow,
            };

            await context.Employees.AddRangeAsync(adminEmployee, managerEmployee, employeeEmployee,
                employee2, employee3, employee4, employee5);
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

            // Seed Module Items for Module 5 (React)
            var module5Items = new List<ModuleItem>
            {
                new ModuleItem
                {
                    Id = Guid.NewGuid(),
                    ModuleId = module5.Id,
                    Title = "React Setup and JSX",
                    OrderIndex = 1,
                    Descriptions = "Getting started with React",
                    Url = "https://react.dev",
                    IsFinalSubmission = false,
                },
                new ModuleItem
                {
                    Id = Guid.NewGuid(),
                    ModuleId = module5.Id,
                    Title = "State and Props",
                    OrderIndex = 2,
                    Descriptions = "Component state management",
                    Url = "https://react.dev/learn",
                    IsFinalSubmission = false,
                },
                new ModuleItem
                {
                    Id = Guid.NewGuid(),
                    ModuleId = module5.Id,
                    Title = "Hooks Deep Dive",
                    OrderIndex = 3,
                    Descriptions = "Master React hooks",
                    Url = "https://react.dev/reference",
                    IsFinalSubmission = false,
                },
                new ModuleItem
                {
                    Id = Guid.NewGuid(),
                    ModuleId = module5.Id,
                    Title = "Final React Application",
                    OrderIndex = 4,
                    Descriptions = "Build complete React app",
                    Url = "https://github.com",
                    IsFinalSubmission = true,
                },
            };

            await context.ModuleItems.AddRangeAsync(module1Items);
            await context.ModuleItems.AddRangeAsync(module2Items);
            await context.ModuleItems.AddRangeAsync(module3Items);
            await context.ModuleItems.AddRangeAsync(module4Items);
            await context.ModuleItems.AddRangeAsync(module5Items);
            await context.SaveChangesAsync();

            var moduleItems = module1Items; // Keep reference for enrollment seeding

            // Seed Enrollments with diverse statuses
            // Employee 1 (John) - OnGoing in Module 1
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

            // Employee 2 (Jessica) - Completed Module 2
            var enrollment2 = new Enrollment
            {
                Id = Guid.NewGuid(),
                AccountId = employee2Account.Id,
                ModuleId = module2.Id,
                StartDate = DateTime.UtcNow.AddDays(-20),
                TargetDate = DateTime.UtcNow.AddDays(-6),
                Status = EnrollmentStatus.Completed,
                CurrentProgress = 100,
                CreatedAt = DateTime.UtcNow.AddDays(-20),
                CompletedDate = DateTime.UtcNow.AddDays(-5),
            };

            // Employee 2 (Jessica) - OnGoing in Module 4
            var enrollment3 = new Enrollment
            {
                Id = Guid.NewGuid(),
                AccountId = employee2Account.Id,
                ModuleId = module4.Id,
                StartDate = DateTime.UtcNow.AddDays(-5),
                TargetDate = DateTime.UtcNow.AddDays(5),
                Status = EnrollmentStatus.OnGoing,
                CurrentProgress = 66,
                CreatedAt = DateTime.UtcNow.AddDays(-5),
            };

            // Employee 3 (Christopher) - Paused in Module 5
            var enrollment4 = new Enrollment
            {
                Id = Guid.NewGuid(),
                AccountId = employee3Account.Id,
                ModuleId = module5.Id,
                StartDate = DateTime.UtcNow.AddDays(-10),
                TargetDate = DateTime.UtcNow.AddDays(2),
                Status = EnrollmentStatus.Paused,
                CurrentProgress = 50,
                CreatedAt = DateTime.UtcNow.AddDays(-10),
            };

            // Employee 3 (Christopher) - Completed Module 1
            var enrollment5 = new Enrollment
            {
                Id = Guid.NewGuid(),
                AccountId = employee3Account.Id,
                ModuleId = module1.Id,
                StartDate = DateTime.UtcNow.AddDays(-15),
                TargetDate = DateTime.UtcNow.AddDays(-8),
                Status = EnrollmentStatus.Completed,
                CurrentProgress = 100,
                CreatedAt = DateTime.UtcNow.AddDays(-15),
                CompletedDate = DateTime.UtcNow.AddDays(-7),
            };

            // Employee 5 (Daniel) - OnGoing in Module 2
            var enrollment6 = new Enrollment
            {
                Id = Guid.NewGuid(),
                AccountId = employee5Account.Id,
                ModuleId = module2.Id,
                StartDate = DateTime.UtcNow.AddDays(-8),
                TargetDate = DateTime.UtcNow.AddDays(6),
                Status = EnrollmentStatus.OnGoing,
                CurrentProgress = 20,
                CreatedAt = DateTime.UtcNow.AddDays(-8),
            };

            // Employee 4 (Amanda) - Completed Module 5
            var enrollment7 = new Enrollment
            {
                Id = Guid.NewGuid(),
                AccountId = employee4Account.Id,
                ModuleId = module5.Id,
                StartDate = DateTime.UtcNow.AddDays(-18),
                TargetDate = DateTime.UtcNow.AddDays(-6),
                Status = EnrollmentStatus.Completed,
                CurrentProgress = 100,
                CreatedAt = DateTime.UtcNow.AddDays(-18),
                CompletedDate = DateTime.UtcNow.AddDays(-5),
            };

            await context.Enrollments.AddRangeAsync(enrollment, enrollment2, enrollment3,
                enrollment4, enrollment5, enrollment6, enrollment7);
            await context.SaveChangesAsync();

            // Seed Enrollment Items for all enrollments
            // Enrollment 1 items (1/3 completed)
            var enrollment1Items = new List<EnrollmentItem>
            {
                new EnrollmentItem
                {
                    Id = Guid.NewGuid(),
                    EnrollmentId = enrollment.Id,
                    ModuleItemId = module1Items[0].Id,
                    IsCompleted = true,
                    EvidenceUrl = "https://github.com/johndoe/aspnet-setup",
                    CompletedAt = DateTime.UtcNow.AddDays(-2),
                },
                new EnrollmentItem
                {
                    Id = Guid.NewGuid(),
                    EnrollmentId = enrollment.Id,
                    ModuleItemId = module1Items[1].Id,
                    IsCompleted = false,
                },
                new EnrollmentItem
                {
                    Id = Guid.NewGuid(),
                    EnrollmentId = enrollment.Id,
                    ModuleItemId = module1Items[2].Id,
                    IsCompleted = false,
                },
            };

            // Enrollment 2 items (All completed)
            var enrollment2Items = new List<EnrollmentItem>
            {
                new EnrollmentItem
                {
                    Id = Guid.NewGuid(),
                    EnrollmentId = enrollment2.Id,
                    ModuleItemId = module2Items[0].Id,
                    IsCompleted = true,
                    EvidenceUrl = "https://github.com/jessica/async-project",
                    CompletedAt = DateTime.UtcNow.AddDays(-15),
                },
                new EnrollmentItem
                {
                    Id = Guid.NewGuid(),
                    EnrollmentId = enrollment2.Id,
                    ModuleItemId = module2Items[1].Id,
                    IsCompleted = true,
                    EvidenceUrl = "https://github.com/jessica/linq-exercises",
                    CompletedAt = DateTime.UtcNow.AddDays(-12),
                },
                new EnrollmentItem
                {
                    Id = Guid.NewGuid(),
                    EnrollmentId = enrollment2.Id,
                    ModuleItemId = module2Items[2].Id,
                    IsCompleted = true,
                    EvidenceUrl = "https://github.com/jessica/delegates-demo",
                    CompletedAt = DateTime.UtcNow.AddDays(-10),
                },
                new EnrollmentItem
                {
                    Id = Guid.NewGuid(),
                    EnrollmentId = enrollment2.Id,
                    ModuleItemId = module2Items[3].Id,
                    IsCompleted = true,
                    EvidenceUrl = "https://github.com/jessica/generics-project",
                    CompletedAt = DateTime.UtcNow.AddDays(-7),
                },
                new EnrollmentItem
                {
                    Id = Guid.NewGuid(),
                    EnrollmentId = enrollment2.Id,
                    ModuleItemId = module2Items[4].Id,
                    IsCompleted = true,
                    EvidenceUrl = "https://github.com/jessica/final-csharp",
                    CompletedAt = DateTime.UtcNow.AddDays(-5),
                },
            };

            // Enrollment 3 items (2/3 completed)
            var enrollment3Items = new List<EnrollmentItem>
            {
                new EnrollmentItem
                {
                    Id = Guid.NewGuid(),
                    EnrollmentId = enrollment3.Id,
                    ModuleItemId = module4Items[0].Id,
                    IsCompleted = true,
                    EvidenceUrl = "https://github.com/jessica/ef-basics",
                    CompletedAt = DateTime.UtcNow.AddDays(-3),
                },
                new EnrollmentItem
                {
                    Id = Guid.NewGuid(),
                    EnrollmentId = enrollment3.Id,
                    ModuleItemId = module4Items[1].Id,
                    IsCompleted = true,
                    EvidenceUrl = "https://github.com/jessica/ef-migrations",
                    CompletedAt = DateTime.UtcNow.AddDays(-1),
                },
                new EnrollmentItem
                {
                    Id = Guid.NewGuid(),
                    EnrollmentId = enrollment3.Id,
                    ModuleItemId = module4Items[2].Id,
                    IsCompleted = false,
                },
            };

            // Enrollment 4 items (2/4 completed - Paused)
            var enrollment4Items = new List<EnrollmentItem>
            {
                new EnrollmentItem
                {
                    Id = Guid.NewGuid(),
                    EnrollmentId = enrollment4.Id,
                    ModuleItemId = module5Items[0].Id,
                    IsCompleted = true,
                    EvidenceUrl = "https://github.com/christopher/react-setup",
                    CompletedAt = DateTime.UtcNow.AddDays(-8),
                },
                new EnrollmentItem
                {
                    Id = Guid.NewGuid(),
                    EnrollmentId = enrollment4.Id,
                    ModuleItemId = module5Items[1].Id,
                    IsCompleted = true,
                    EvidenceUrl = "https://github.com/christopher/react-state",
                    CompletedAt = DateTime.UtcNow.AddDays(-6),
                },
                new EnrollmentItem
                {
                    Id = Guid.NewGuid(),
                    EnrollmentId = enrollment4.Id,
                    ModuleItemId = module5Items[2].Id,
                    IsCompleted = false,
                },
                new EnrollmentItem
                {
                    Id = Guid.NewGuid(),
                    EnrollmentId = enrollment4.Id,
                    ModuleItemId = module5Items[3].Id,
                    IsCompleted = false,
                },
            };

            // Enrollment 5 items (All completed)
            var enrollment5Items = new List<EnrollmentItem>
            {
                new EnrollmentItem
                {
                    Id = Guid.NewGuid(),
                    EnrollmentId = enrollment5.Id,
                    ModuleItemId = module1Items[0].Id,
                    IsCompleted = true,
                    EvidenceUrl = "https://github.com/christopher/aspnet-env",
                    CompletedAt = DateTime.UtcNow.AddDays(-13),
                },
                new EnrollmentItem
                {
                    Id = Guid.NewGuid(),
                    EnrollmentId = enrollment5.Id,
                    ModuleItemId = module1Items[1].Id,
                    IsCompleted = true,
                    EvidenceUrl = "https://github.com/christopher/first-api",
                    CompletedAt = DateTime.UtcNow.AddDays(-10),
                },
                new EnrollmentItem
                {
                    Id = Guid.NewGuid(),
                    EnrollmentId = enrollment5.Id,
                    ModuleItemId = module1Items[2].Id,
                    IsCompleted = true,
                    EvidenceUrl = "https://github.com/christopher/aspnet-final",
                    CompletedAt = DateTime.UtcNow.AddDays(-7),
                },
            };

            // Enrollment 6 items (1/5 completed)
            var enrollment6Items = new List<EnrollmentItem>
            {
                new EnrollmentItem
                {
                    Id = Guid.NewGuid(),
                    EnrollmentId = enrollment6.Id,
                    ModuleItemId = module2Items[0].Id,
                    IsCompleted = true,
                    EvidenceUrl = "https://github.com/daniel/async-basics",
                    CompletedAt = DateTime.UtcNow.AddDays(-6),
                },
                new EnrollmentItem
                {
                    Id = Guid.NewGuid(),
                    EnrollmentId = enrollment6.Id,
                    ModuleItemId = module2Items[1].Id,
                    IsCompleted = false,
                },
                new EnrollmentItem
                {
                    Id = Guid.NewGuid(),
                    EnrollmentId = enrollment6.Id,
                    ModuleItemId = module2Items[2].Id,
                    IsCompleted = false,
                },
                new EnrollmentItem
                {
                    Id = Guid.NewGuid(),
                    EnrollmentId = enrollment6.Id,
                    ModuleItemId = module2Items[3].Id,
                    IsCompleted = false,
                },
                new EnrollmentItem
                {
                    Id = Guid.NewGuid(),
                    EnrollmentId = enrollment6.Id,
                    ModuleItemId = module2Items[4].Id,
                    IsCompleted = false,
                },
            };

            // Enrollment 7 items (All completed - Amanda)
            var enrollment7Items = new List<EnrollmentItem>
            {
                new EnrollmentItem
                {
                    Id = Guid.NewGuid(),
                    EnrollmentId = enrollment7.Id,
                    ModuleItemId = module5Items[0].Id,
                    IsCompleted = true,
                    EvidenceUrl = "https://github.com/amanda/react-setup",
                    CompletedAt = DateTime.UtcNow.AddDays(-16),
                },
                new EnrollmentItem
                {
                    Id = Guid.NewGuid(),
                    EnrollmentId = enrollment7.Id,
                    ModuleItemId = module5Items[1].Id,
                    IsCompleted = true,
                    EvidenceUrl = "https://github.com/amanda/react-state-props",
                    CompletedAt = DateTime.UtcNow.AddDays(-13),
                },
                new EnrollmentItem
                {
                    Id = Guid.NewGuid(),
                    EnrollmentId = enrollment7.Id,
                    ModuleItemId = module5Items[2].Id,
                    IsCompleted = true,
                    EvidenceUrl = "https://github.com/amanda/react-hooks",
                    CompletedAt = DateTime.UtcNow.AddDays(-9),
                },
                new EnrollmentItem
                {
                    Id = Guid.NewGuid(),
                    EnrollmentId = enrollment7.Id,
                    ModuleItemId = module5Items[3].Id,
                    IsCompleted = true,
                    EvidenceUrl = "https://github.com/amanda/react-final-app",
                    CompletedAt = DateTime.UtcNow.AddDays(-5),
                },
            };

            await context.EnrollmentItems.AddRangeAsync(enrollment1Items);
            await context.EnrollmentItems.AddRangeAsync(enrollment2Items);
            await context.EnrollmentItems.AddRangeAsync(enrollment3Items);
            await context.EnrollmentItems.AddRangeAsync(enrollment4Items);
            await context.EnrollmentItems.AddRangeAsync(enrollment5Items);
            await context.EnrollmentItems.AddRangeAsync(enrollment6Items);
            await context.EnrollmentItems.AddRangeAsync(enrollment7Items);
            await context.SaveChangesAsync();

            // Seed Submissions for completed enrollments
            var submission1 = new Submission
            {
                Id = Guid.NewGuid(),
                EnrollmentId = enrollment2.Id,
                Status = SubmissionStatus.Approved,
                CreatedAt = DateTime.UtcNow.AddDays(-5),
                UpdatedAt = DateTime.UtcNow.AddDays(-4),
            };

            var submission2 = new Submission
            {
                Id = Guid.NewGuid(),
                EnrollmentId = enrollment5.Id,
                Status = SubmissionStatus.Pending,
                CreatedAt = DateTime.UtcNow.AddDays(-7),
            };

            var submission3 = new Submission
            {
                Id = Guid.NewGuid(),
                EnrollmentId = enrollment7.Id,
                Status = SubmissionStatus.Approved,
                CreatedAt = DateTime.UtcNow.AddDays(-6),
                UpdatedAt = DateTime.UtcNow.AddDays(-5),
            };

            await context.Submissions.AddRangeAsync(submission1, submission2, submission3);
            await context.SaveChangesAsync();
        }
    }
}