using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shopping.Domain.Models;

namespace Shopping.Persistence.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ConfigureBaseEntity();

            builder.Property(m => m.Title).HasColumnType("nvarchar").HasMaxLength(200).IsRequired();
            builder.Property(m => m.Description).HasColumnType("nvarchar(max)").IsRequired();
            builder.Property(m => m.Price).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(m => m.Brand).HasColumnType("nvarchar").HasMaxLength(100).IsRequired();
            builder.Property(m => m.Quantity).HasColumnType("int").IsRequired();

            builder.HasOne(m => m.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(m => m.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.ToTable("Products");

            builder.HasData(
                new Product { Id = 1, Title = "Wireless Mouse", Description = "Ergonomic wireless mouse", Price = 19.99m, Brand = "Logitech", Quantity = 100, CategoryId = 1, CreatedAt = new DateTime(2026, 1, 1) },
                new Product { Id = 2, Title = "Cotton T-Shirt", Description = "100% cotton t-shirt", Price = 9.99m, Brand = "Generic", Quantity = 200, CategoryId = 2, CreatedAt = new DateTime(2026, 1, 1) },
                new Product { Id = 3, Title = "Clean Code", Description = "A Handbook of Agile Software Craftsmanship", Price = 29.99m, Brand = "Prentice Hall", Quantity = 50, CategoryId = 3, CreatedAt = new DateTime(2026, 3, 3) }
            );
        }
    }

}
