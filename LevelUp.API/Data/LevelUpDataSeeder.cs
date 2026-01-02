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

            // ---------------------
            // POSITIONS (single division sample)
            // ---------------------
            var positions = new[]
            {
                new Position { Id = Guid.Parse("10000000-0000-0000-0000-000000000001"), Title = "Backend Developer", IsActive = true },
                new Position { Id = Guid.Parse("10000000-0000-0000-0000-000000000002"), Title = "Frontend Developer", IsActive = true },
                new Position { Id = Guid.Parse("10000000-0000-0000-0000-000000000003"), Title = "QA Engineer", IsActive = true },
                new Position { Id = Guid.Parse("10000000-0000-0000-0000-000000000004"), Title = "DevOps Engineer", IsActive = true },
            };

            await context.Positions.AddRangeAsync(positions);
            await context.SaveChangesAsync();

            // ---------------------
            // ACCOUNTS (1 admin, 1 manager, 8 employees)
            // ---------------------
            var admin = new Account
            {
                Id = Guid.Parse("20000000-0000-0000-0000-000000000001"),
                Email = "admin@levelup.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("12341234"),
                Role = UserRole.Admin,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                OtpHash = null,
                OtpExpiresAt = null,
                OtpAttempts = 0,
            };

            var manager = new Account
            {
                Id = Guid.Parse("20000000-0000-0000-0000-000000000002"),
                Email = "manager@levelup.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Manager123!"),
                Role = UserRole.Manager,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                // demo OTP: hashed
                OtpHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                OtpExpiresAt = DateTime.UtcNow.AddMinutes(10),
                OtpAttempts = 0,
            };

            var employees = new[]
            {
                new Account { Id = Guid.Parse("20000000-0000-0000-0000-000000000003"), Email = "john.doe@levelup.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("12341234"), Role = UserRole.Employee, IsActive = true, CreatedAt = DateTime.UtcNow },
                new Account { Id = Guid.Parse("20000000-0000-0000-0000-000000000004"), Email = "jessica.martinez@levelup.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("12341234"), Role = UserRole.Employee, IsActive = true, CreatedAt = DateTime.UtcNow },
                new Account { Id = Guid.Parse("20000000-0000-0000-0000-000000000005"), Email = "christopher.taylor@levelup.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("12341234"), Role = UserRole.Employee, IsActive = true, CreatedAt = DateTime.UtcNow },
                new Account { Id = Guid.Parse("20000000-0000-0000-0000-000000000006"), Email = "amanda.davis@levelup.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("12341234"), Role = UserRole.Employee, IsActive = true, CreatedAt = DateTime.UtcNow },
                new Account { Id = Guid.Parse("20000000-0000-0000-0000-000000000007"), Email = "daniel.thompson@levelup.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("12341234"), Role = UserRole.Employee, IsActive = true, CreatedAt = DateTime.UtcNow },
                new Account { Id = Guid.Parse("20000000-0000-0000-0000-000000000008"), Email = "michael.anderson@levelup.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("12341234"), Role = UserRole.Employee, IsActive = true, CreatedAt = DateTime.UtcNow },
                new Account { Id = Guid.Parse("20000000-0000-0000-0000-000000000009"), Email = "sarah.wilson@levelup.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("12341234"), Role = UserRole.Employee, IsActive = true, CreatedAt = DateTime.UtcNow },
                new Account { Id = Guid.Parse("20000000-0000-0000-0000-000000000010"), Email = "inactive.employee@levelup.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("12341234"), Role = UserRole.Employee, IsActive = false, CreatedAt = DateTime.UtcNow }, // inactive
            };

            await context.Accounts.AddAsync(admin);
            await context.Accounts.AddAsync(manager);
            await context.Accounts.AddRangeAsync(employees);
            await context.SaveChangesAsync();

            // ---------------------
            // EMPLOYEES (link accounts and positions)
            // ---------------------
            var employeeEntities = new List<Employee>
            {
                new Employee { Id = Guid.Parse("30000000-0000-0000-0000-000000000001"), AccountId = admin.Id, FirstName = "Super", LastName = "Admin", PositionId = positions[0].Id, IsIdle = false, CreatedAt = DateTime.UtcNow },
                new Employee { Id = Guid.Parse("30000000-0000-0000-0000-000000000002"), AccountId = manager.Id, FirstName = "Team", LastName = "Manager", PositionId = positions[1 % positions.Length].Id, IsIdle = false, CreatedAt = DateTime.UtcNow },
            };

            var empNames = new[] { ("John", "Doe"), ("Jessica", "Martinez"), ("Christopher", "Taylor"), ("Amanda", "Davis"), ("Daniel", "Thompson"), ("Michael", "Anderson"), ("Sarah", "Wilson"), ("Inactive", "Employee") };
            for (int i = 0; i < employees.Length; i++)
            {
                employeeEntities.Add(new Employee
                {
                    Id = Guid.Parse($"30000000-0000-0000-0000-{(3 + i):000000000000}"),
                    AccountId = employees[i].Id,
                    FirstName = empNames[i].Item1,
                    LastName = empNames[i].Item2,
                    PositionId = positions[i % positions.Length].Id,
                    IsIdle = i >= 5, // last ones idle for demo
                    CreatedAt = DateTime.UtcNow,
                });
            }

            await context.Employees.AddRangeAsync(employeeEntities);
            await context.SaveChangesAsync();

            // ---------------------
            // MODULES (5 modules relevant to division)
            // ---------------------
            var modules = new[]
            {
                new Module { Id = Guid.Parse("40000000-0000-0000-0000-000000000001"), Title = "ASP.NET Core Fundamentals", Description = "Basic ASP.NET Core", EstimatedDays = 7, IsActive = true, CreatedBy = manager.Id, CreatedAt = DateTime.UtcNow },
                new Module { Id = Guid.Parse("40000000-0000-0000-0000-000000000002"), Title = "Advanced C# Programming", Description = "Advanced C# topics", EstimatedDays = 14, IsActive = true, CreatedBy = manager.Id, CreatedAt = DateTime.UtcNow },
                new Module { Id = Guid.Parse("40000000-0000-0000-0000-000000000003"), Title = "Entity Framework Core", Description = "EF Core and DB design", EstimatedDays = 10, IsActive = true, CreatedBy = manager.Id, CreatedAt = DateTime.UtcNow },
                new Module { Id = Guid.Parse("40000000-0000-0000-0000-000000000004"), Title = "React Fundamentals", Description = "Frontend basics", EstimatedDays = 12, IsActive = true, CreatedBy = manager.Id, CreatedAt = DateTime.UtcNow },
                new Module { Id = Guid.Parse("40000000-0000-0000-0000-000000000005"), Title = "DevOps with Azure", Description = "CI/CD & infra", EstimatedDays = 15, IsActive = false, CreatedBy = manager.Id, CreatedAt = DateTime.UtcNow }, // inactive
            };

            await context.Modules.AddRangeAsync(modules);
            await context.SaveChangesAsync();

            // ---------------------
            // MODULE ITEMS (3-4 items per module)
            // ---------------------
            var moduleItems = new List<ModuleItem>();
            int itemIndex = 1;
            var moduleItemTitles = new Dictionary<Guid, string[]>
            {
                { modules[0].Id, new[] { "Setup", "Build API", "Auth & Final" } },
                { modules[1].Id, new[] { "Async & Concurrency", "LINQ", "Patterns & Final" } },
                { modules[2].Id, new[] { "EF Core Basics", "Migrations", "Relationships" } },
                { modules[3].Id, new[] { "React Setup", "State & Hooks", "Final Project" } },
                { modules[4].Id, new[] { "Azure Basics", "CI/CD", "Monitoring" } },
            };

            foreach (var module in modules)
            {
                var titles = moduleItemTitles[module.Id];
                for (int i = 0; i < titles.Length; i++)
                {
                    moduleItems.Add(new ModuleItem { Id = Guid.Parse($"50000000-0000-0000-0000-{itemIndex:000000000000}"), ModuleId = module.Id, Title = titles[i], OrderIndex = i + 1, IsFinalSubmission = i == titles.Length - 1 });
                    itemIndex++;
                }
            }

            await context.ModuleItems.AddRangeAsync(moduleItems);
            await context.SaveChangesAsync();

            // ---------------------
            // ENROLLMENTS + ITEMS + SUBMISSIONS (focused scenarios)
            // ---------------------
            var enrollments = new List<Enrollment>();
            int enrollmentIndex = 1;

            // John: OnGoing, 2/3
            var e1 = new Enrollment { Id = Guid.Parse($"60000000-0000-0000-0000-{enrollmentIndex:000000000000}"), AccountId = employees[0].Id, ModuleId = modules[0].Id, Status = EnrollmentStatus.OnGoing, StartDate = DateTime.UtcNow.AddDays(-5), TargetDate = DateTime.UtcNow.AddDays(2), CreatedAt = DateTime.UtcNow.AddDays(-5) };
            enrollmentIndex++;

            // Jessica: Completed, all items done
            var e2 = new Enrollment { Id = Guid.Parse($"60000000-0000-0000-0000-{enrollmentIndex:000000000000}"), AccountId = employees[1].Id, ModuleId = modules[1].Id, Status = EnrollmentStatus.Completed, StartDate = DateTime.UtcNow.AddDays(-30), TargetDate = DateTime.UtcNow.AddDays(-10), CompletedDate = DateTime.UtcNow.AddDays(-10), CreatedAt = DateTime.UtcNow.AddDays(-30) };
            enrollmentIndex++;

            // Christopher: Paused, 1/3
            var e3 = new Enrollment { Id = Guid.Parse($"60000000-0000-0000-0000-{enrollmentIndex:000000000000}"), AccountId = employees[2].Id, ModuleId = modules[3].Id, Status = EnrollmentStatus.Paused, StartDate = DateTime.UtcNow.AddDays(-12), TargetDate = DateTime.UtcNow.AddDays(3), CreatedAt = DateTime.UtcNow.AddDays(-12) };
            enrollmentIndex++;

            // Amanda: OnGoing, 0/3 (just started)
            var e4 = new Enrollment { Id = Guid.Parse($"60000000-0000-0000-0000-{enrollmentIndex:000000000000}"), AccountId = employees[3].Id, ModuleId = modules[2].Id, Status = EnrollmentStatus.OnGoing, StartDate = DateTime.UtcNow.AddDays(-1), TargetDate = DateTime.UtcNow.AddDays(9), CreatedAt = DateTime.UtcNow.AddDays(-1) };
            enrollmentIndex++;

            // Michael: Completed on inactive module (edge case)
            var e5 = new Enrollment { Id = Guid.Parse($"60000000-0000-0000-0000-{enrollmentIndex:000000000000}"), AccountId = employees[5].Id, ModuleId = modules[4].Id, Status = EnrollmentStatus.Completed, StartDate = DateTime.UtcNow.AddDays(-40), TargetDate = DateTime.UtcNow.AddDays(-20), CompletedDate = DateTime.UtcNow.AddDays(-20), CreatedAt = DateTime.UtcNow.AddDays(-40) };
            enrollmentIndex++;

            enrollments.AddRange(new[] { e1, e2, e3, e4, e5 });
            await context.Enrollments.AddRangeAsync(enrollments);
            await context.SaveChangesAsync();

            // Create enrollment items and set progress
            var enrollmentItems = new List<EnrollmentItem>();
            int enrollItemIdx = 1;

            var progress = new Dictionary<Guid, int>
            {
                { e1.Id, 2 }, // John 2/3
                { e2.Id, 3 }, // Jessica 3/3
                { e3.Id, 1 }, // Christopher 1/3
                { e4.Id, 0 }, // Amanda 0/3
                { e5.Id, 3 }, // Michael 3/3 (module inactive but completed earlier)
            };

            foreach (var enroll in enrollments)
            {
                var itemsForModule = moduleItems.Where(mi => mi.ModuleId == enroll.ModuleId).OrderBy(mi => mi.OrderIndex).ToList();
                int done = progress.ContainsKey(enroll.Id) ? progress[enroll.Id] : 0;
                for (int i = 0; i < itemsForModule.Count; i++)
                {
                    enrollmentItems.Add(new EnrollmentItem { Id = Guid.Parse($"70000000-0000-0000-0000-{enrollItemIdx:000000000000}"), EnrollmentId = enroll.Id, ModuleItemId = itemsForModule[i].Id, IsCompleted = i < done, CompletedAt = i < done ? DateTime.UtcNow.AddDays(-(done - i)) : null });
                    enrollItemIdx++;
                }

                // set CurrentProgress
                enroll.CurrentProgress = done;
                context.Enrollments.Update(enroll);
            }

            await context.EnrollmentItems.AddRangeAsync(enrollmentItems);
            await context.SaveChangesAsync();

            // Submissions: only use Pending/Approved/Rejected
            var submissions = new List<Submission>
            {
                new Submission { Id = Guid.Parse("80000000-0000-0000-0000-000000000001"), EnrollmentId = e2.Id, Status = SubmissionStatus.Approved, ManagerFeedback = "Excellent implementation of patterns.", EstimatedDays = 14, CreatedAt = DateTime.UtcNow.AddDays(-9) },
                new Submission { Id = Guid.Parse("80000000-0000-0000-0000-000000000002"), EnrollmentId = e3.Id, Status = SubmissionStatus.Pending, ManagerFeedback = null, EstimatedDays = 12, CreatedAt = DateTime.UtcNow.AddDays(-2) },
                new Submission { Id = Guid.Parse("80000000-0000-0000-0000-000000000003"), EnrollmentId = e5.Id, Status = SubmissionStatus.Rejected, ManagerFeedback = "Please add more infra tests.", EstimatedDays = 15, CreatedAt = DateTime.UtcNow.AddDays(-18) },
            };

            await context.Submissions.AddRangeAsync(submissions);
            await context.SaveChangesAsync();

            // Update employee idle status based on enrollments
            foreach (var emp in employeeEntities.Where(e => e.AccountId != admin.Id && e.AccountId != manager.Id))
            {
                var hasOngoing = enrollments.Any(en => en.AccountId == emp.AccountId && en.Status == EnrollmentStatus.OnGoing);
                var hasRecentCompleted = enrollments.Any(en => en.AccountId == emp.AccountId && en.Status == EnrollmentStatus.Completed && en.CompletedDate.HasValue && (DateTime.UtcNow - en.CompletedDate.Value).TotalDays <= 30);
                emp.IsIdle = !hasOngoing && !hasRecentCompleted;
            }

            context.Employees.UpdateRange(employeeEntities);
            await context.SaveChangesAsync();
        }
    }
}