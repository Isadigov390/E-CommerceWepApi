using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shopping.Domain.Entities.Accounts;

namespace Shopping.Persistence.Configurations
{
    public class EmailVerificationConfiguration : IEntityTypeConfiguration<EmailVerification>
    {
        public void Configure(EntityTypeBuilder<EmailVerification> builder)
        {
            builder.ConfigureBaseEntity();

            builder.Property(m => m.CodeHash).HasColumnType("nvarchar").HasMaxLength(128).IsRequired();
            builder.Property(m => m.ExpiresAt).HasColumnType("datetime").IsRequired();
            builder.Property(m => m.AttemptCount).HasColumnType("int").IsRequired().HasDefaultValue(0);
            builder.Property(m => m.UsedAt).HasColumnType("datetime");

            builder.HasOne(m => m.User)
                .WithMany(u => u.EmailVerifications)
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(m => new { m.UserId, m.ExpiresAt });

            builder.ToTable("EmailVerifications");
        }
    }
}
