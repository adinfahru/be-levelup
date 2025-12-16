using LevelUp.API.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LevelUp.API.Data.Configurations
{
    public class SubmissionConfiguration : IEntityTypeConfiguration<Submission>
    {
        public void Configure(EntityTypeBuilder<Submission> builder)
        {
            builder.ToTable("submissions");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.Id).IsRequired().HasColumnName("id");

            builder.Property(s => s.EnrollmentId).IsRequired().HasColumnName("enrollment_id");

            builder
                .Property(s => s.Status)
                .IsRequired()
                .HasColumnName("status")
                .HasConversion<string>();

            builder.Property(s => s.Notes)
                .HasColumnName("notes")
                .HasMaxLength(2000)      
                .IsRequired(false);

            builder.Property(s => s.ManagerFeedback)
                .HasColumnName("manager_feedback")
                .HasMaxLength(2000)     
                .IsRequired(false);

            builder
                .Property(s => s.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()")
                .HasColumnName("created_at");

            builder.Property(s => s.UpdatedAt).HasColumnName("updated_at");

            // Relationships
            builder
                .HasOne(s => s.Enrollment)
                .WithMany(e => e.Submissions)
                .HasForeignKey(s => s.EnrollmentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
