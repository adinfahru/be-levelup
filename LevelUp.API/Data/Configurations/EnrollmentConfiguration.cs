using LevelUp.API.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LevelUp.API.Data.Configurations
{
    public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
    {
        public void Configure(EntityTypeBuilder<Enrollment> builder)
        {
            builder.ToTable("enrollments");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id).IsRequired().HasColumnName("id");
            builder.Property(e => e.AccountId).IsRequired().HasColumnName("account_id");
            builder.Property(e => e.ModuleId).IsRequired().HasColumnName("module_id");
            builder.Property(e => e.StartDate).IsRequired().HasColumnName("start_date");
            builder.Property(e => e.TargetDate).IsRequired().HasColumnName("target_date");
            builder.Property(e => e.CompletedDate).HasColumnName("completed_date");

            builder
                .Property(e => e.Status)
                .IsRequired()
                .HasColumnName("status")
                .HasConversion<string>();

            builder
                .Property(e => e.CurrentProgress)
                .HasDefaultValue(0)
                .HasColumnName("current_progress");

            builder
                .Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()")
                .HasColumnName("created_at");

            builder.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            // Relationships
            builder
                .HasOne(e => e.Account)
                .WithMany(a => a.Enrollments)
                .HasForeignKey(e => e.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne(e => e.Module)
                .WithMany(m => m.Enrollments)
                .HasForeignKey(e => e.ModuleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasMany(e => e.Items)
                .WithOne(ei => ei.Enrollment)
                .HasForeignKey(ei => ei.EnrollmentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasMany(e => e.Submissions)
                .WithOne(s => s.Enrollment)
                .HasForeignKey(s => s.EnrollmentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
