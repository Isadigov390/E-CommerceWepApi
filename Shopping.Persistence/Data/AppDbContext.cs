using Microsoft.EntityFrameworkCore;
using Shopping.Domain.Entities.Interfaces;
using Shopping.Domain.Models;

namespace Shopping.Persistence.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var changes = this.ChangeTracker.Entries<IAuditableEntity>();

            if (changes != null)
            {
                foreach (var entry in changes.Where(m => m.State == EntityState.Added
                || m.State == EntityState.Modified
                || m.State == EntityState.Deleted))
                {
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            entry.Entity.CreatedAt = DateTime.UtcNow;
                            break;
                        case EntityState.Modified:
                            entry.Property(m => m.CreatedBy).IsModified = false;
                            entry.Property(m => m.CreatedAt).IsModified = false;
                            entry.Entity.LastModifiedAt = DateTime.UtcNow;
                            break;
                        case EntityState.Deleted:
                            entry.State = EntityState.Modified;
                            entry.Property(m => m.CreatedBy).IsModified = false;
                            entry.Property(m => m.CreatedAt).IsModified = false;
                            entry.Property(m => m.LastModifiedBy).IsModified = false;
                            entry.Property(m => m.LastModifiedAt).IsModified = false;
                            entry.Entity.DeletedAt = DateTime.UtcNow;
                            break;
                        default:
                            break;
                    }
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}

