using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shopping.Domain.Models;

namespace Shopping.Persistence.Configurations
{
    public class ContactConfiguration : IEntityTypeConfiguration<Contact>
    {
        public void Configure(EntityTypeBuilder<Contact> builder)
        {
            builder.ConfigureBaseEntity();

            builder.Property(m => m.PhoneNumber).HasColumnType("nvarchar").HasMaxLength(20).IsRequired();

            builder.HasOne(m => m.Person)
                .WithMany(p => p.Contacts)
                .HasForeignKey(m => m.PersonId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.ToTable("Contacts");

            builder.HasData(
                new Contact { Id = 1, PhoneNumber = "+1-555-0101", PersonId = 1, CreatedAt = new DateTime(2026, 1, 1) },
                new Contact { Id = 2, PhoneNumber = "+1-555-0102", PersonId = 2, CreatedAt = new DateTime(2026, 1, 1) }
            );
        }
    }

}
