using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shopping.Domain.Entities.Accounts;

namespace Shopping.Persistence.Configurations
{
    public class PasswordResetTokenConfiguration : IEntityTypeConfiguration<PasswordResetToken>
    {
        public void Configure(EntityTypeBuilder<PasswordResetToken> builder)
        {
            builder.ConfigureBaseEntity();

            builder.Property(x => x.TokenHash).HasColumnType("nvarchar").HasMaxLength(64).IsRequired();
            builder.Property(x => x.ExpiresAtUtc).HasColumnType("datetime2").IsRequired();
            builder.Property(x => x.UsedAtUtc).HasColumnType("datetime2").IsRequired(false);

            builder.HasOne(x => x.User).WithMany(x => x.PasswordResetTokens).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.TokenHash).HasFilter("[DeletedAt] IS NULL").IsUnique();
            builder.HasIndex(x => x.UserId);

            builder.ToTable("PasswordResetTokens");
        }
    }
}