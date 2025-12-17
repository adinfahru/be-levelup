using LevelUp.API.Entity;

namespace LevelUp.API.Data
{
    /*
     * ===== FIXED GUID REFERENCE =====
     * All IDs are fixed for consistent testing without manual replacement
     * 
     * POSITIONS (10000000-...)
     * - 10000000-0000-0000-0000-000000000001 = Fullstack .NET Developer
     * - 10000000-0000-0000-0000-000000000002 = Fullstack Java Developer
     * - 10000000-0000-0000-0000-000000000003 = Quality Assurance
     * 
     * ACCOUNTS (20000000-...)
     * - 20000000-0000-0000-0000-000000000001 = Admin (admin@levelup.com)
     * - 20000000-0000-0000-0000-000000000002 = Manager (manager@levelup.com)
     * - 20000000-0000-0000-0000-000000000003 = John Doe (employee@levelup.com)
     * - 20000000-0000-0000-0000-000000000004 = Jessica Martinez (employee2@levelup.com)
     * - 20000000-0000-0000-0000-000000000005 = Christopher Taylor (employee3@levelup.com)
     * - 20000000-0000-0000-0000-000000000006 = Amanda Davis (employee4@levelup.com)
     * - 20000000-0000-0000-0000-000000000007 = Daniel Thompson (employee5@levelup.com)
     * - 20000000-0000-0000-0000-000000000008 = Michael Anderson (employee6@levelup.com) - NEVER ENROLLED
     * - 20000000-0000-0000-0000-000000000009 = Sarah Wilson (employee7@levelup.com) - NEVER ENROLLED
     * 
     * EMPLOYEES (30000000-...)
     * - 30000000-0000-0000-0000-000000000001 = Admin Employee
     * - 30000000-0000-0000-0000-000000000002 = Manager Employee
     * - 30000000-0000-0000-0000-000000000003 = John Doe
     * - 30000000-0000-0000-0000-000000000004 = Jessica Martinez
     * - 30000000-0000-0000-0000-000000000005 = Christopher Taylor
     * - 30000000-0000-0000-0000-000000000006 = Amanda Davis
     * - 30000000-0000-0000-0000-000000000007 = Daniel Thompson
     * - 30000000-0000-0000-0000-000000000008 = Michael Anderson - NEVER ENROLLED
     * - 30000000-0000-0000-0000-000000000009 = Sarah Wilson - NEVER ENROLLED
     * 
     * MODULES (40000000-...)
     * - 40000000-0000-0000-0000-000000000001 = Introduction to ASP.NET Core (7 days)
     * - 40000000-0000-0000-0000-000000000002 = Advanced C# Programming (14 days)
     * - 40000000-0000-0000-0000-000000000003 = Microservices Architecture (21 days) - INACTIVE
     * - 40000000-0000-0000-0000-000000000004 = Database Design with Entity Framework (10 days)
     * - 40000000-0000-0000-0000-000000000005 = React Fundamentals (12 days)
     * 
     * MODULE ITEMS (50000000-...)
     * Module 1 (ASP.NET Core):
     *   - 50000000-0000-0000-0000-000000000001 = Setup Development Environment
     *   - 50000000-0000-0000-0000-000000000002 = Create First API
     *   - 50000000-0000-0000-0000-000000000003 = Final Project Submission
     * Module 2 (Advanced C#):
     *   - 50000000-0000-0000-0000-000000000004 = Async/Await Deep Dive
     *   - 50000000-0000-0000-0000-000000000005 = LINQ Mastery
     *   - 50000000-0000-0000-0000-000000000006 = Delegates and Events
     *   - 50000000-0000-0000-0000-000000000007 = Generics and Collections
     *   - 50000000-0000-0000-0000-000000000008 = Final Project
     * Module 3 (Microservices):
     *   - 50000000-0000-0000-0000-000000000009 = Microservices Introduction
     *   - 50000000-0000-0000-0000-000000000010 = Docker Containerization
     *   - 50000000-0000-0000-0000-000000000011 = Final Microservices Project
     * Module 4 (EF Core):
     *   - 50000000-0000-0000-0000-000000000012 = EF Core Basics
     *   - 50000000-0000-0000-0000-000000000013 = Database Migrations
     *   - 50000000-0000-0000-0000-000000000014 = Final Database Project
     * Module 5 (React):
     *   - 50000000-0000-0000-0000-000000000015 = React Setup and JSX
     *   - 50000000-0000-0000-0000-000000000016 = State and Props
     *   - 50000000-0000-0000-0000-000000000017 = Hooks Deep Dive
     *   - 50000000-0000-0000-0000-000000000018 = Final React Application
     * 
     * ENROLLMENTS (60000000-...)
     * - 60000000-0000-0000-0000-000000000001 = John (OnGoing) in Module 1 (1/3 completed)
     * - 60000000-0000-0000-0000-000000000002 = Jessica (Completed) in Module 2 (5/5 completed)
     * - 60000000-0000-0000-0000-000000000003 = Jessica (OnGoing) in Module 4 (2/3 completed)
     * - 60000000-0000-0000-0000-000000000004 = Christopher (Paused) in Module 5 (2/4 completed)
     * - 60000000-0000-0000-0000-000000000005 = Christopher (Completed) in Module 1 (3/3 completed)
     * - 60000000-0000-0000-0000-000000000006 = Daniel (OnGoing) in Module 2 (1/5 completed)
     * - 60000000-0000-0000-0000-000000000007 = Amanda (Completed) in Module 5 (4/4 completed)
     * 
     * ENROLLMENT ITEMS (70000000-...)
     * Enrollment 1 (John - Module 1): 70000000-...-0001 to 0003
     * Enrollment 2 (Jessica - Module 2): 70000000-...-0004 to 0008
     * Enrollment 3 (Jessica - Module 4): 70000000-...-0009 to 0011
     * Enrollment 4 (Christopher - Module 5): 70000000-...-0012 to 0015
     * Enrollment 5 (Christopher - Module 1): 70000000-...-0016 to 0018
     * Enrollment 6 (Daniel - Module 2): 70000000-...-0019 to 0023
     * Enrollment 7 (Amanda - Module 5): 70000000-...-0024 to 0027
     * 
     * SUBMISSIONS (80000000-...)
     * - 80000000-0000-0000-0000-000000000001 = Jessica's Module 2 (Approved)
     * - 80000000-0000-0000-0000-000000000002 = Christopher's Module 1 (Pending)
     * - 80000000-0000-0000-0000-000000000003 = Amanda's Module 5 (Approved)
     */
    public class LevelUpDataSeeder
    {
        public static async Task SeedAsync(LevelUpDbContext context)
        {
            if (context.Accounts.Any())
                return;

            // =====================
            // POSITIONS (Fixed GUIDs 10000000-...)
            // =====================
            var positions = new[]
            {
                new Position { Id = Guid.Parse("10000000-0000-0000-0000-000000000001"), Title = "Backend Developer", IsActive = true },
                new Position { Id = Guid.Parse("10000000-0000-0000-0000-000000000002"), Title = "Frontend Developer", IsActive = true },
                new Position { Id = Guid.Parse("10000000-0000-0000-0000-000000000003"), Title = "QA Engineer", IsActive = true },
            };

            await context.Positions.AddRangeAsync(positions);
            await context.SaveChangesAsync();

            // =====================
            // ACCOUNTS (Fixed GUIDs 20000000-...)
            // =====================
            var admin = new Account
            {
                Id = Guid.Parse("20000000-0000-0000-0000-000000000001"),
                Email = "admin@levelup.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                Role = UserRole.Admin,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
            };

            var manager = new Account
            {
                Id = Guid.Parse("20000000-0000-0000-0000-000000000002"),
                Email = "manager@levelup.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Manager123!"),
                Role = UserRole.Manager,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
            };

            var employees = new[]
            {
                new Account { Id = Guid.Parse("20000000-0000-0000-0000-000000000003"), Email = "employee@levelup.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Employee123!"), Role = UserRole.Employee, IsActive = true, CreatedAt = DateTime.UtcNow },
                new Account { Id = Guid.Parse("20000000-0000-0000-0000-000000000004"), Email = "employee2@levelup.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Employee123!"), Role = UserRole.Employee, IsActive = true, CreatedAt = DateTime.UtcNow },
                new Account { Id = Guid.Parse("20000000-0000-0000-0000-000000000005"), Email = "employee3@levelup.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Employee123!"), Role = UserRole.Employee, IsActive = true, CreatedAt = DateTime.UtcNow },
                new Account { Id = Guid.Parse("20000000-0000-0000-0000-000000000006"), Email = "employee4@levelup.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Employee123!"), Role = UserRole.Employee, IsActive = true, CreatedAt = DateTime.UtcNow },
                new Account { Id = Guid.Parse("20000000-0000-0000-0000-000000000007"), Email = "employee5@levelup.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Employee123!"), Role = UserRole.Employee, IsActive = true, CreatedAt = DateTime.UtcNow },
                new Account { Id = Guid.Parse("20000000-0000-0000-0000-000000000008"), Email = "employee6@levelup.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Employee123!"), Role = UserRole.Employee, IsActive = true, CreatedAt = DateTime.UtcNow },
                new Account { Id = Guid.Parse("20000000-0000-0000-0000-000000000009"), Email = "employee7@levelup.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Employee123!"), Role = UserRole.Employee, IsActive = true, CreatedAt = DateTime.UtcNow },
            };

            await context.Accounts.AddAsync(admin);
            await context.Accounts.AddAsync(manager);
            await context.Accounts.AddRangeAsync(employees);
            await context.SaveChangesAsync();

            // =====================
            // EMPLOYEES (Fixed GUIDs 30000000-...)
            // =====================
            var employeeEntities = new List<Employee>();

            // Admin Employee
            employeeEntities.Add(new Employee
            {
                Id = Guid.Parse("30000000-0000-0000-0000-000000000001"),
                AccountId = admin.Id,
                FirstName = "Super",
                LastName = "Admin",
                PositionId = positions[0].Id,
                CreatedAt = DateTime.UtcNow,
            });

            // Manager Employee
            employeeEntities.Add(new Employee
            {
                Id = Guid.Parse("30000000-0000-0000-0000-000000000002"),
                AccountId = manager.Id,
                FirstName = "Manager",
                LastName = "One",
                PositionId = positions[0].Id,
                CreatedAt = DateTime.UtcNow,
            });

            // Regular Employees
            var empNames = new[] { "John", "Jessica", "Christopher", "Amanda", "Daniel", "Michael", "Sarah" };
            for (int i = 0; i < employees.Length; i++)
            {
                employeeEntities.Add(new Employee
                {
                    Id = Guid.Parse($"30000000-0000-0000-0000-00000000000{3 + i}"),
                    AccountId = employees[i].Id,
                    FirstName = empNames[i],
                    LastName = "Employee",
                    PositionId = positions[i % positions.Length].Id,
                    IsIdle = i >= 5, // Last 2 are idle (never enrolled)
                    CreatedAt = DateTime.UtcNow,
                });
            }

            await context.Employees.AddRangeAsync(employeeEntities);
            await context.SaveChangesAsync();

            // =====================
            // MODULES (Fixed GUIDs 40000000-...)
            // =====================
            var modules = new[]
            {
                new Module
                {
                    Id = Guid.Parse("40000000-0000-0000-0000-000000000001"),
                    Title = "ASP.NET Core Fundamentals",
                    Description = "Learn ASP.NET Core basics",
                    EstimatedDays = 7,
                    IsActive = true,
                    CreatedBy = manager.Id,
                    CreatedAt = DateTime.UtcNow,
                },
                new Module
                {
                    Id = Guid.Parse("40000000-0000-0000-0000-000000000002"),
                    Title = "Advanced C# Programming",
                    Description = "Master advanced C# concepts",
                    EstimatedDays = 14,
                    IsActive = true,
                    CreatedBy = manager.Id,
                    CreatedAt = DateTime.UtcNow,
                },
                new Module
                {
                    Id = Guid.Parse("40000000-0000-0000-0000-000000000003"),
                    Title = "Microservices Architecture",
                    Description = "Build scalable microservices",
                    EstimatedDays = 21,
                    IsActive = false, // INACTIVE
                    CreatedBy = manager.Id,
                    CreatedAt = DateTime.UtcNow,
                },
                new Module
                {
                    Id = Guid.Parse("40000000-0000-0000-0000-000000000004"),
                    Title = "Entity Framework Core",
                    Description = "Database design with EF Core",
                    EstimatedDays = 10,
                    IsActive = true,
                    CreatedBy = manager.Id,
                    CreatedAt = DateTime.UtcNow,
                },
                new Module
                {
                    Id = Guid.Parse("40000000-0000-0000-0000-000000000005"),
                    Title = "React Fundamentals",
                    Description = "Front-end development with React",
                    EstimatedDays = 12,
                    IsActive = true,
                    CreatedBy = manager.Id,
                    CreatedAt = DateTime.UtcNow,
                },
            };

            await context.Modules.AddRangeAsync(modules);
            await context.SaveChangesAsync();

            // =====================
            // MODULE ITEMS (Fixed GUIDs 50000000-...)
            // =====================
            var moduleItems = new List<ModuleItem>();
            int itemIndex = 1;

            foreach (var module in modules)
            {
                for (int i = 1; i <= 3; i++)
                {
                    moduleItems.Add(new ModuleItem
                    {
                        Id = Guid.Parse($"50000000-0000-0000-0000-{itemIndex:000000000000}"),
                        ModuleId = module.Id,
                        Title = i < 3 ? $"Task {i}" : "Final Submission",
                        OrderIndex = i,
                        IsFinalSubmission = i == 3,
                    });
                    itemIndex++;
                }
            }

            await context.ModuleItems.AddRangeAsync(moduleItems);
            await context.SaveChangesAsync();

            // =====================
            // ENROLLMENTS + ENROLLMENT ITEMS + SUBMISSIONS (Fixed GUIDs 60000000-... & 70000000-... & 80000000-...)
            // =====================
            int enrollmentIndex = 1;
            int enrollItemIndex = 1;
            int submissionIndex = 1;

            // Employee 0 (John): OnGoing Module 1, 1/3 completed
            var enroll1 = new Enrollment
            {
                Id = Guid.Parse($"60000000-0000-0000-0000-{enrollmentIndex:000000000000}"),
                AccountId = employees[0].Id,
                ModuleId = modules[0].Id,
                Status = EnrollmentStatus.OnGoing,
                StartDate = DateTime.UtcNow.AddDays(-7),
                TargetDate = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow,
            };
            enrollmentIndex++;
            employeeEntities[2].IsIdle = true; // John is idle (ongoing)

            // Employee 1 (Jessica): OnGoing Module 4, 2/3 completed + Completed Module 2
            var enroll2 = new Enrollment
            {
                Id = Guid.Parse($"60000000-0000-0000-0000-{enrollmentIndex:000000000000}"),
                AccountId = employees[1].Id,
                ModuleId = modules[3].Id,
                Status = EnrollmentStatus.OnGoing,
                StartDate = DateTime.UtcNow.AddDays(-10),
                TargetDate = DateTime.UtcNow.AddDays(5),
                CreatedAt = DateTime.UtcNow,
            };
            enrollmentIndex++;
            employeeEntities[3].IsIdle = true; // Jessica is idle (ongoing)

            var enroll2b = new Enrollment
            {
                Id = Guid.Parse($"60000000-0000-0000-0000-{enrollmentIndex:000000000000}"),
                AccountId = employees[1].Id,
                ModuleId = modules[1].Id,
                Status = EnrollmentStatus.Completed,
                StartDate = DateTime.UtcNow.AddDays(-30),
                TargetDate = DateTime.UtcNow.AddDays(-5),
                CompletedDate = DateTime.UtcNow.AddDays(-5),
                CreatedAt = DateTime.UtcNow,
            };
            enrollmentIndex++;

            // Employee 2 (Christopher): Paused Module 5, 2/3 completed + Completed Module 1
            var enroll3 = new Enrollment
            {
                Id = Guid.Parse($"60000000-0000-0000-0000-{enrollmentIndex:000000000000}"),
                AccountId = employees[2].Id,
                ModuleId = modules[4].Id,
                Status = EnrollmentStatus.Paused,
                StartDate = DateTime.UtcNow.AddDays(-15),
                TargetDate = DateTime.UtcNow.AddDays(10),
                CreatedAt = DateTime.UtcNow,
            };
            enrollmentIndex++;
            employeeEntities[4].IsIdle = false; // Christopher is NOT idle (paused)

            var enroll3b = new Enrollment
            {
                Id = Guid.Parse($"60000000-0000-0000-0000-{enrollmentIndex:000000000000}"),
                AccountId = employees[2].Id,
                ModuleId = modules[0].Id,
                Status = EnrollmentStatus.Completed,
                StartDate = DateTime.UtcNow.AddDays(-40),
                TargetDate = DateTime.UtcNow.AddDays(-15),
                CompletedDate = DateTime.UtcNow.AddDays(-15),
                CreatedAt = DateTime.UtcNow,
            };
            enrollmentIndex++;

            // Employee 3 (Amanda): Completed Module 5 only (Idle, never had ongoing)
            var enroll4 = new Enrollment
            {
                Id = Guid.Parse($"60000000-0000-0000-0000-{enrollmentIndex:000000000000}"),
                AccountId = employees[3].Id,
                ModuleId = modules[4].Id,
                Status = EnrollmentStatus.Completed,
                StartDate = DateTime.UtcNow.AddDays(-20),
                TargetDate = DateTime.UtcNow.AddDays(-5),
                CompletedDate = DateTime.UtcNow.AddDays(-5),
                CreatedAt = DateTime.UtcNow,
            };
            enrollmentIndex++;
            employeeEntities[5].IsIdle = true; // Amanda is idle (completed only)

            // Employee 4 (Daniel): OnGoing Module 2, 1/3 completed
            var enroll5 = new Enrollment
            {
                Id = Guid.Parse($"60000000-0000-0000-0000-{enrollmentIndex:000000000000}"),
                AccountId = employees[4].Id,
                ModuleId = modules[1].Id,
                Status = EnrollmentStatus.OnGoing,
                StartDate = DateTime.UtcNow.AddDays(-5),
                TargetDate = DateTime.UtcNow.AddDays(10),
                CreatedAt = DateTime.UtcNow,
            };
            enrollmentIndex++;
            employeeEntities[6].IsIdle = true; // Daniel is idle (ongoing)

            var allEnrollments = new[] { enroll1, enroll2, enroll2b, enroll3, enroll3b, enroll4, enroll5 };
            await context.Enrollments.AddRangeAsync(allEnrollments);
            await context.SaveChangesAsync();

            // Create Enrollment Items
            var enrollmentItemsMap = new Dictionary<Guid, List<ModuleItem>>
            {
                { enroll1.Id, moduleItems.Where(mi => mi.ModuleId == modules[0].Id).ToList() },
                { enroll2.Id, moduleItems.Where(mi => mi.ModuleId == modules[3].Id).ToList() },
                { enroll2b.Id, moduleItems.Where(mi => mi.ModuleId == modules[1].Id).ToList() },
                { enroll3.Id, moduleItems.Where(mi => mi.ModuleId == modules[4].Id).ToList() },
                { enroll3b.Id, moduleItems.Where(mi => mi.ModuleId == modules[0].Id).ToList() },
                { enroll4.Id, moduleItems.Where(mi => mi.ModuleId == modules[4].Id).ToList() },
                { enroll5.Id, moduleItems.Where(mi => mi.ModuleId == modules[1].Id).ToList() },
            };

            var completedCounts = new Dictionary<Guid, int>
            {
                { enroll1.Id, 1 }, // 1/3
                { enroll2.Id, 2 }, // 2/3
                { enroll2b.Id, 3 }, // 3/3 (completed)
                { enroll3.Id, 2 }, // 2/3
                { enroll3b.Id, 3 }, // 3/3 (completed)
                { enroll4.Id, 3 }, // 3/3 (completed)
                { enroll5.Id, 1 }, // 1/3
            };

            foreach (var enrollment in allEnrollments)
            {
                var items = enrollmentItemsMap[enrollment.Id];
                int completedCount = completedCounts[enrollment.Id];

                for (int i = 0; i < items.Count; i++)
                {
                    await context.EnrollmentItems.AddAsync(new EnrollmentItem
                    {
                        Id = Guid.Parse($"70000000-0000-0000-0000-{enrollItemIndex:000000000000}"),
                        EnrollmentId = enrollment.Id,
                        ModuleItemId = items[i].Id,
                        IsCompleted = i < completedCount,
                    });
                    enrollItemIndex++;
                }
            }

            // Create Submissions
            await context.Submissions.AddAsync(new Submission
            {
                Id = Guid.Parse($"80000000-0000-0000-0000-{submissionIndex:000000000000}"),
                EnrollmentId = enroll2b.Id,
                Status = SubmissionStatus.Approved,
                ManagerFeedback = "Excellent work!",
                EstimatedDays = 14,
                CreatedAt = DateTime.UtcNow,
            });
            submissionIndex++;

            await context.Submissions.AddAsync(new Submission
            {
                Id = Guid.Parse($"80000000-0000-0000-0000-{submissionIndex:000000000000}"),
                EnrollmentId = enroll3b.Id,
                Status = SubmissionStatus.Approved,
                ManagerFeedback = "Great job!",
                EstimatedDays = 7,
                CreatedAt = DateTime.UtcNow,
            });
            submissionIndex++;

            await context.Submissions.AddAsync(new Submission
            {
                Id = Guid.Parse($"80000000-0000-0000-0000-{submissionIndex:000000000000}"),
                EnrollmentId = enroll4.Id,
                Status = SubmissionStatus.Approved,
                ManagerFeedback = "Perfect execution!",
                EstimatedDays = 12,
                CreatedAt = DateTime.UtcNow,
            });

            // Update employees after setting IsIdle
            context.Employees.UpdateRange(employeeEntities);

            await context.SaveChangesAsync();
        }
    }
}
