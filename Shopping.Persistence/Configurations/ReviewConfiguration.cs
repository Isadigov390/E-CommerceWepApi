using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shopping.Domain.Models;

namespace Shopping.Persistence.Configurations
{
    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder.ConfigureBaseEntity();

            builder.Property(m => m.Stars).HasColumnType("int").IsRequired();
            builder.Property(m => m.Comment).HasColumnType("nvarchar").HasMaxLength(1000);

            builder.HasCheckConstraint("CK_Review_Stars_Range", "[Stars] >= 1 AND [Stars] <= 5");

            builder.HasOne(m => m.Product)
                .WithMany(p => p.Reviews)
                .HasForeignKey(m => m.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(m => m.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.ToTable("Reviews");

            builder.HasData(
                new Review { Id = 1, Stars = 5, Comment = "Great mouse, very responsive!", ProductId = 1, UserId = 1, CreatedAt = new DateTime(2026, 1, 1) },
                new Review { Id = 2, Stars = 4, Comment = "Good quality shirt, fits well.", ProductId = 2, UserId = 2, CreatedAt = new DateTime(2026, 1, 1) },
                new Review { Id = 3, Stars = 5, Comment = "Must-read for every developer.", ProductId = 3, UserId = 1, CreatedAt = new DateTime(2026, 1, 1) }
            );
        }
    }
}
