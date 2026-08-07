using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shopping.Domain.Entities.Accounts;

namespace Shopping.Persistence.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ConfigureBaseEntity();

            builder.Property(m => m.TokenHash).HasColumnType("nvarchar").HasMaxLength(64).IsRequired();
            builder.Property(m => m.ExpiresAtUtc).HasColumnType("datetime").IsRequired();
            builder.Property(m => m.RevokedAtUtc).HasColumnType("datetime");

            builder.HasOne(m => m.User).WithMany(u => u.RefreshTokens).HasForeignKey(m => m.UserId).OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(m => m.TokenHash).HasFilter("[DeletedAt] IS NULL").IsUnique();
            builder.HasIndex(m => m.UserId);

            builder.ToTable("RefreshTokens");
        }
    }
}
