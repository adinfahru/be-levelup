using LevelUp.API.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LevelUp.API.Data.Configurations
{
    public class EnrollmentItemConfiguration : IEntityTypeConfiguration<EnrollmentItem>
    {
        public void Configure(EntityTypeBuilder<EnrollmentItem> builder)
        {
            builder.ToTable("enrollment_items");

            builder.HasKey(ei => ei.Id);

            builder.Property(ei => ei.Id).IsRequired().HasColumnName("id");
            builder.Property(ei => ei.EnrollmentId).IsRequired().HasColumnName("enrollment_id");
            builder.Property(ei => ei.ModuleItemId).IsRequired().HasColumnName("module_item_id");
            builder
                .Property(ei => ei.IsCompleted)
                .HasDefaultValue(false)
                .HasColumnName("is_completed");
            builder.Property(ei => ei.Feedback).HasMaxLength(1000).HasColumnName("feedback");
            builder.Property(ei => ei.EvidenceUrl).HasMaxLength(500).HasColumnName("evidence_url");
            builder.Property(ei => ei.CompletedAt).HasColumnName("completed_at");

            // Relationships
            builder
                .HasOne(ei => ei.Enrollment)
                .WithMany(e => e.Items)
                .HasForeignKey(ei => ei.EnrollmentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne(ei => ei.ModuleItem)
                .WithMany(mi => mi.EnrollmentItems)
                .HasForeignKey(ei => ei.ModuleItemId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
