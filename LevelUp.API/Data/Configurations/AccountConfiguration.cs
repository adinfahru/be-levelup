using LevelUp.API.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LevelUp.API.Data.Configurations
{
    public class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.ToTable("accounts");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id).IsRequired().HasColumnName("id");
            builder.Property(a => a.Email).IsRequired().HasMaxLength(255).HasColumnName("email");

            builder
                .Property(a => a.PasswordHash)
                .IsRequired()
                .HasMaxLength(512)
                .HasColumnName("password_hash");

            builder
                .Property(a => a.Role)
                .IsRequired()
                .HasColumnName("role")
                .HasConversion<string>();

            // OTP stored as hashed string in "otp" column for backward compatibility
            builder.Property(a => a.OtpHash).HasColumnName("otp").HasMaxLength(512).IsRequired(false);
            builder.Property(a => a.OtpExpiresAt).HasColumnName("otp_expires_at");
            builder.Property(a => a.OtpAttempts).HasColumnName("otp_attempts").HasDefaultValue(0);

            builder.Property(a => a.IsActive).HasDefaultValue(true).HasColumnName("is_active");

            builder
                .Property(a => a.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()")
                .HasColumnName("created_at");

            builder.Property(a => a.UpdatedAt).HasColumnName("updated_at");

            // Relationships
            builder
                .HasOne(a => a.Employee)
                .WithOne(e => e.Account)
                .HasForeignKey<Employee>(e => e.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasMany(a => a.Enrollments)
                .WithOne(e => e.Account)
                .HasForeignKey(e => e.AccountId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
