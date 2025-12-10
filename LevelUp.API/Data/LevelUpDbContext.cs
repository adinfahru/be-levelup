using LevelUp.API.Data.Configurations;
using LevelUp.API.Entity;
using Microsoft.EntityFrameworkCore;

namespace LevelUp.API.Data
{
    public class LevelUpDbContext : DbContext
    {
        public LevelUpDbContext(DbContextOptions<LevelUpDbContext> options)
            : base(options) { }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<ModuleItem> ModuleItems { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<EnrollmentItem> EnrollmentItems { get; set; }
        public DbSet<Submission> Submissions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply all entity configurations
            modelBuilder.ApplyConfiguration(new AccountConfiguration());
            modelBuilder.ApplyConfiguration(new EmployeeConfiguration());
            modelBuilder.ApplyConfiguration(new PositionConfiguration());
            modelBuilder.ApplyConfiguration(new ModuleConfiguration());
            modelBuilder.ApplyConfiguration(new ModuleItemConfiguration());
            modelBuilder.ApplyConfiguration(new EnrollmentConfiguration());
            modelBuilder.ApplyConfiguration(new EnrollmentItemConfiguration());
            modelBuilder.ApplyConfiguration(new SubmissionConfiguration());
        }
    }
}
