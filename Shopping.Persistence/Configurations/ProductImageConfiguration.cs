using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shopping.Domain.Models;

namespace Shopping.Persistence.Configurations
{
    public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
    {
        public void Configure(EntityTypeBuilder<ProductImage> builder)
        {
            builder.ConfigureBaseEntity();
            builder.Property(m => m.FilePath).HasColumnType("nvarchar").HasMaxLength(300).IsRequired();
            builder.Property(m => m.FileName).HasColumnType("nvarchar").HasMaxLength(200).IsRequired();
            builder.Property(m => m.IsMain).HasColumnType("bit").IsRequired();
            builder.HasOne(m => m.Product)
                .WithMany(p => p.ProductImages)
                .HasForeignKey(m => m.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasIndex(m => new { m.ProductId, m.IsMain })
                .HasFilter("[IsMain] = 1")
                .IsUnique();

            builder.ToTable("ProductImages");

            builder.HasData(
                new ProductImage { Id = 1, FilePath = "/images/products/", FileName = "wireless-mouse.jpg", IsMain = true, ProductId = 1, CreatedAt = new DateTime(2026, 1, 1) },
                new ProductImage { Id = 2, FilePath = "/images/products/", FileName = "tshirt.jpg", IsMain = true, ProductId = 2, CreatedAt = new DateTime(2026, 1, 1) },
                new ProductImage { Id = 3, FilePath = "/images/products/", FileName = "clean-code.jpg", IsMain = true, ProductId = 3, CreatedAt = new DateTime(2026, 1, 1) }
            );

        }
    }

}
