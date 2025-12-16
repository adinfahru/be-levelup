using LevelUp.API.Entity;

namespace LevelUp.API.Data
{
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
                Id = Guid.NewGuid(),
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
