using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shopping.Domain.Models.Common;

namespace Shopping.Persistence.Configurations
{
    static class TypeConfigurationHelper
    {
        public static EntityTypeBuilder<T> ConfigureBaseEntity<T>(this EntityTypeBuilder<T> builder)
            where T : BaseEntity
        {
            builder.HasKey(m => m.Id);
            builder.Property(m => m.Id).HasColumnType("int").UseIdentityColumn(1, 1);
            builder.Property(m => m.CreatedAt).HasColumnType("datetime");
            builder.Property(m => m.CreatedBy).HasColumnType("int");
            builder.Property(m => m.LastModifiedAt).HasColumnType("datetime");
            builder.Property(m => m.LastModifiedBy).HasColumnType("int");
            builder.Property(m => m.DeletedAt).HasColumnType("datetime");
            builder.Property(m => m.DeletedBy).HasColumnType("int");

            builder.HasQueryFilter(m => m.DeletedAt == null);
            return builder;
        }
    }
}
