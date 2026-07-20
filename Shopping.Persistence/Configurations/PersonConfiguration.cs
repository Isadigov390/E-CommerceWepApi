using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shopping.Domain.Models;

namespace Shopping.Persistence.Configurations
{
    public class PersonConfiguration : IEntityTypeConfiguration<Person>
    {
        public void Configure(EntityTypeBuilder<Person> builder)
        {
            builder.ConfigureBaseEntity();

            builder.Property(m => m.Name).HasColumnType("nvarchar").HasMaxLength(100).IsRequired();
            builder.Property(m => m.Surname).HasColumnType("nvarchar").HasMaxLength(100).IsRequired();
            builder.Property(m => m.FatherName).HasColumnType("nvarchar").HasMaxLength(100).IsRequired();
            builder.Property(m => m.DateOfBirth).HasColumnType("date").IsRequired();

            builder.HasMany(m => m.Contacts)
                .WithOne(c => c.Person)
                .HasForeignKey(c => c.PersonId)
                .OnDelete(DeleteBehavior.Cascade);

            // Person <-> User one-to-one is configured from the User side (see UserConfiguration),
            // since User holds the FK (PersonId).

            builder.ToTable("Persons");

            builder.HasData(
                new Person { Id = 1, Name = "John", Surname = "Doe", FatherName = "Michael", DateOfBirth = new DateTime(1990, 5, 10), CreatedAt = new DateTime(2026, 1, 1) },
                new Person { Id = 2, Name = "Jane", Surname = "Smith", FatherName = "Robert", DateOfBirth = new DateTime(1995, 8, 22), CreatedAt = new DateTime(2026, 1, 1) }
            );
        }
    }
}
