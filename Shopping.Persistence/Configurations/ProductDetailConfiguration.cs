using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shopping.Domain.Enums;
using Shopping.Domain.Models;

namespace Shopping.Persistence.Configurations
{
    public class ProductDetailConfiguration : IEntityTypeConfiguration<ProductDetail>
    {
        public void Configure(EntityTypeBuilder<ProductDetail> builder)
        {
            builder.ConfigureBaseEntity();

            builder.Property(m => m.SKU).HasColumnType("nvarchar").HasMaxLength(50).IsRequired();
            builder.Property(m => m.Warranty).HasColumnType("int").IsRequired();
            builder.Property(m => m.Discount).HasColumnType("int");

            builder.HasOne(m => m.Product)
                .WithOne(p => p.ProductDetail)
                .HasForeignKey<ProductDetail>(m => m.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(m => m.ProductId).IsUnique();

            builder.ToTable("ProductDetails");

            builder.HasData(
                new ProductDetail { Id = 1, SKU = "ELEC-0001", Discount = 0, Warranty = WarrantyType.NoWarranty, ProductId = 1, CreatedAt = new DateTime(2026, 1, 1) },
                new ProductDetail { Id = 2, SKU = "CLTH-0001", Discount = 10, Warranty = WarrantyType.OneYear, ProductId = 2, CreatedAt = new DateTime(2026, 1, 1) },
                new ProductDetail { Id = 3, SKU = "BOOK-0001", Discount = 5, Warranty = WarrantyType.TwoYear, ProductId = 3, CreatedAt = new DateTime(2026, 1, 1) }
            );
        }
    }
}
