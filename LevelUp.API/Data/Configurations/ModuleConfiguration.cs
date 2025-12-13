using LevelUp.API.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LevelUp.API.Data.Configurations
{
    public class ModuleConfiguration : IEntityTypeConfiguration<Module>
    {
        public void Configure(EntityTypeBuilder<Module> builder)
        {
            builder.ToTable("modules");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Id).IsRequired().HasColumnName("id");
            builder.Property(m => m.Title).IsRequired().HasMaxLength(200).HasColumnName("title");
            builder.Property(m => m.Description).HasMaxLength(1000).HasColumnName("description");
            builder.Property(m => m.EstimatedDays).IsRequired().HasColumnName("estimated_days");
            builder.Property(m => m.IsActive).HasDefaultValue(true).HasColumnName("is_active");
            builder.Property(m => m.CreatedBy).IsRequired().HasColumnName("created_by");
            builder
                .Property(m => m.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()")
                .HasColumnName("created_at");
            builder.Property(m => m.UpdatedAt).HasColumnName("updated_at");
            builder.Property(m => m.DeletedAt).HasColumnName("deleted_at");

            // Relationships
            builder
                .HasOne(m => m.Creator)
                .WithMany(a => a.CreatedModules)
                .HasForeignKey(m => m.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasMany(m => m.Items)
                .WithOne(mi => mi.Module)
                .HasForeignKey(mi => mi.ModuleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasMany(m => m.Enrollments)
                .WithOne(e => e.Module)
                .HasForeignKey(e => e.ModuleId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
