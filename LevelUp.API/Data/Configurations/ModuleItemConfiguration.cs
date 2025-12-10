using LevelUp.API.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LevelUp.API.Data.Configurations
{
    public class ModuleItemConfiguration : IEntityTypeConfiguration<ModuleItem>
    {
        public void Configure(EntityTypeBuilder<ModuleItem> builder)
        {
            builder.ToTable("module_items");

            builder.HasKey(mi => mi.Id);

            builder.Property(mi => mi.Id).IsRequired().HasColumnName("id");
            builder.Property(mi => mi.ModuleId).IsRequired().HasColumnName("module_id");
            builder.Property(mi => mi.Title).IsRequired().HasMaxLength(200).HasColumnName("title");
            builder.Property(mi => mi.OrderIndex).IsRequired().HasColumnName("order_index");
            builder
                .Property(mi => mi.Descriptions)
                .HasMaxLength(1000)
                .HasColumnName("descriptions");
            builder.Property(mi => mi.Url).HasMaxLength(500).HasColumnName("url");
            builder
                .Property(mi => mi.IsFinalSubmission)
                .HasDefaultValue(false)
                .HasColumnName("is_final_submission");

            // Relationships
            builder
                .HasOne(mi => mi.Module)
                .WithMany(m => m.Items)
                .HasForeignKey(mi => mi.ModuleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasMany(mi => mi.EnrollmentItems)
                .WithOne(ei => ei.ModuleItem)
                .HasForeignKey(ei => ei.ModuleItemId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
