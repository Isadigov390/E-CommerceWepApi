using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shopping.Domain.Models;

namespace Shopping.Persistence.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ConfigureBaseEntity();

            builder.Property(m => m.Name).HasColumnType("nvarchar").HasMaxLength(100).IsRequired();

            builder.ToTable("Categories");

            builder.HasData(
                new Category { Id = 1, Name = "Electronics", CreatedAt = new DateTime(2026, 1, 1) },
                new Category { Id = 2, Name = "Clothing", CreatedAt = new DateTime(2026, 1, 1) },
                new Category { Id = 3, Name = "Books", CreatedAt = new DateTime(2026, 1, 1) }
            );
        }
    }
}
