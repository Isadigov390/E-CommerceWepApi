using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shopping.Domain.Entities.Accounts;

namespace Shopping.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ConfigureBaseEntity();

            builder.Property(m => m.Name).HasColumnType("nvarchar").HasMaxLength(100).IsRequired();
            builder.Property(m => m.Surname).HasColumnType("nvarchar").HasMaxLength(100).IsRequired();
            builder.Property(m => m.Email).HasColumnType("nvarchar").HasMaxLength(256).IsRequired();
            builder.Property(m => m.PasswordHash).HasColumnType("nvarchar").HasMaxLength(255).IsRequired();
            builder.Property(m => m.EmailConfirmed).HasColumnType("bit").IsRequired().HasDefaultValue(false);


            builder.HasIndex(m => m.Email)
                .HasFilter("[DeletedAt] IS NULL")
                .IsUnique();

            builder.ToTable("Users");
        }
    }
}
