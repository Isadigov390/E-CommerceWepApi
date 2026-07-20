using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shopping.Domain.Models;

namespace Shopping.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ConfigureBaseEntity();

            builder.Property(m => m.Email).HasColumnType("nvarchar").HasMaxLength(200).IsRequired();
            builder.Property(m => m.Username).HasColumnType("nvarchar").HasMaxLength(100).IsRequired();
            builder.Property(m => m.Password).HasColumnType("nvarchar").HasMaxLength(300).IsRequired();
            builder.Property(m => m.ProfileImagePath).HasColumnType("nvarchar").HasMaxLength(300);

            builder.HasOne(m => m.Person)
                .WithOne(p => p.User)
                .HasForeignKey<User>(m => m.PersonId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(m => m.PersonId).IsUnique();
            builder.HasIndex(m => m.Email).IsUnique();
            builder.HasIndex(m => m.Username).IsUnique();

            builder.ToTable("Users");

            builder.HasData(
                new User { Id = 1, PersonId = 1, Email = "john.doe@example.com", Username = "johndoe", Password = "AQAAAAIAAYagAAAAEExampleSeedHash01==", CreatedAt = new DateTime(2026, 1, 1) },
                new User { Id = 2, PersonId = 2, Email = "jane.smith@example.com", Username = "janesmith", Password = "AQAAAAIAAYagAAAAEExampleSeedHash02==", CreatedAt = new DateTime(2026, 1, 1) }
            );
        }
    }

}
