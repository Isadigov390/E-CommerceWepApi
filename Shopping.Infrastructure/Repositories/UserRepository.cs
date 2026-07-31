using Microsoft.EntityFrameworkCore;
using Shopping.Domain.Entities.Accounts;
using Shopping.Domain.Interfaces;
using Shopping.Persistence;
using Shopping.Persistence.Data;

namespace Shopping.Infrastructure.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            var normalized = email.Trim().ToLowerInvariant();

            return await _appDbContext.Set<User>()
                .FirstOrDefaultAsync(u => u.Email == normalized);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            var normalized = email.Trim().ToLowerInvariant();

            return await _appDbContext.Set<User>()
                .AnyAsync(u => u.Email == normalized);
        }
    }
}
