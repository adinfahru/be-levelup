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
            // POSITIONS
            // =====================
            var positions = new[]
            {
                new Position { Id = Guid.NewGuid(), Title = "Backend Developer", IsActive = true },
                new Position { Id = Guid.NewGuid(), Title = "Frontend Developer", IsActive = true },
                new Position { Id = Guid.NewGuid(), Title = "QA Engineer", IsActive = true },
            };

            await context.Positions.AddRangeAsync(positions);
            await context.SaveChangesAsync();

            // =====================
            // ACCOUNTS
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

            var managers = new[]
            {
                new Account { Id = Guid.NewGuid(), Email = "manager1@levelup.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Manager123!"), Role = UserRole.Manager, IsActive = true, CreatedAt = DateTime.UtcNow },
                new Account { Id = Guid.NewGuid(), Email = "manager2@levelup.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Manager123!"), Role = UserRole.Manager, IsActive = true, CreatedAt = DateTime.UtcNow },
                new Account { Id = Guid.NewGuid(), Email = "manager3@levelup.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Manager123!"), Role = UserRole.Manager, IsActive = true, CreatedAt = DateTime.UtcNow },
            };

            var employees = new[]
            {
                ("john@levelup.com","John","Doe"),
                ("sarah@levelup.com","Sarah","Wati"),
                ("andi@levelup.com","Andi","Saputra"),
                ("rina@levelup.com","Rina","Amelia"),
                ("dimas@levelup.com","Dimas","Santoso"),
                ("nabila@levelup.com","Nabila","Putri"),
                ("imam@levelup.com","Imam","Zuhdi"),
                ("farhan@levelup.com","Farhan","Akbar"),
            }
            .Select(x => new Account
            {
                Id = Guid.NewGuid(),
                Email = x.Item1,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Employee123!"),
                Role = UserRole.Employee,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
            })
            .ToList();

            await context.Accounts.AddAsync(admin);
            await context.Accounts.AddRangeAsync(managers);
            await context.Accounts.AddRangeAsync(employees);
            await context.SaveChangesAsync();

            // =====================
            // EMPLOYEES
            // =====================
            var employeeEntities = new List<Employee>();

            employeeEntities.Add(new Employee
            {
                Id = Guid.NewGuid(),
                AccountId = admin.Id,
                FirstName = "Super",
                LastName = "Admin",
                PositionId = positions[0].Id,
                CreatedAt = DateTime.UtcNow,
            });

            for (int i = 0; i < managers.Length; i++)
            {
                employeeEntities.Add(new Employee
                {
                    Id = Guid.NewGuid(),
                    AccountId = managers[i].Id,
                    FirstName = "Manager",
                    LastName = (i + 1).ToString(),
                    PositionId = positions[0].Id,
                    CreatedAt = DateTime.UtcNow,
                });
            }

            for (int i = 0; i < employees.Count; i++)
            {
                employeeEntities.Add(new Employee
                {
                    Id = Guid.NewGuid(),
                    AccountId = employees[i].Id,
                    FirstName = employees[i].Email!.Split('@')[0],
                    LastName = "Employee",
                    PositionId = positions[i % positions.Length].Id,
                    CreatedAt = DateTime.UtcNow,
                });
            }

            await context.Employees.AddRangeAsync(employeeEntities);
            await context.SaveChangesAsync();

            // =====================
            // MODULES + ITEMS
            // =====================
            Module CreateModule(Guid managerId, string title)
            {
                var module = new Module
                {
                    Id = Guid.NewGuid(),
                    Title = title,
                    EstimatedDays = 7,
                    IsActive = true,
                    CreatedBy = managerId,
                    CreatedAt = DateTime.UtcNow,
                };

                context.Modules.Add(module);

                context.ModuleItems.AddRange(
                    new ModuleItem
                    {
                        Id = Guid.NewGuid(),
                        ModuleId = module.Id,
                        Title = "Core Task",
                        OrderIndex = 1,
                        IsFinalSubmission = false,
                    },
                    new ModuleItem
                    {
                        Id = Guid.NewGuid(),
                        ModuleId = module.Id,
                        Title = "Final Submission",
                        OrderIndex = 2,
                        IsFinalSubmission = true,
                    }
                );

                return module;
            }

            var m1Module1 = CreateModule(managers[0].Id, "ASP.NET Bootcamp");
            var m1Module2 = CreateModule(managers[0].Id, "Clean Architecture");
            var m2Module = CreateModule(managers[1].Id, "React Basic");
            var m3Module = CreateModule(managers[2].Id, "Testing Automation");

            await context.SaveChangesAsync();

            // =====================
            // ENROLLMENT + SUBMISSION (FIXED)
            // =====================
            async Task CreateEnrollment(
                Guid accountId,
                Module module,
                bool completeAllItems,
                SubmissionStatus? submissionStatus = null,
                int? estimatedDays = null
            )
            {
                var enrollment = new Enrollment
                {
                    Id = Guid.NewGuid(),
                    AccountId = accountId,
                    ModuleId = module.Id,
                    Status = EnrollmentStatus.OnGoing,
                    StartDate = DateTime.UtcNow.AddDays(-7),
                    TargetDate = DateTime.UtcNow.AddDays(7),
                    CreatedAt = DateTime.UtcNow,
                };

                await context.Enrollments.AddAsync(enrollment);

                var moduleItems = context.ModuleItems
                    .Where(mi => mi.ModuleId == module.Id)
                    .ToList();

                foreach (var item in moduleItems)
                {
                    await context.EnrollmentItems.AddAsync(new EnrollmentItem
                    {
                        Id = Guid.NewGuid(),
                        EnrollmentId = enrollment.Id,
                        ModuleItemId = item.Id,
                        IsCompleted = completeAllItems
                    });
                }

                if (submissionStatus.HasValue)
                {
                    await context.Submissions.AddAsync(new Submission
                    {
                        Id = Guid.NewGuid(),
                        EnrollmentId = enrollment.Id,
                        Status = submissionStatus.Value,
                        ManagerFeedback =
                            submissionStatus == SubmissionStatus.Rejected
                                ? "Masih perlu revisi"
                                : null,
                        EstimatedDays = estimatedDays,
                        CreatedAt = DateTime.UtcNow,
                    });
                }
            }

            // =====================
            // DATA YANG MASUK TABLE SUBMISSION
            // =====================
            await CreateEnrollment(employees[0].Id, m1Module1, true, SubmissionStatus.Pending);
            await CreateEnrollment(employees[1].Id, m1Module1, true, SubmissionStatus.Rejected, 3);
            await CreateEnrollment(employees[2].Id, m1Module2, true, SubmissionStatus.Approved);
            await CreateEnrollment(employees[3].Id, m2Module, true, SubmissionStatus.Pending);

            // =====================
            // DATA YANG TIDAK MASUK (BELUM FINAL)
            // =====================
            await CreateEnrollment(employees[4].Id, m1Module2, false);
            await CreateEnrollment(employees[5].Id, m3Module, false);

            await context.SaveChangesAsync();
        }
    }
}