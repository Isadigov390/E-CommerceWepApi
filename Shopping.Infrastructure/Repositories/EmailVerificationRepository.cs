using Microsoft.EntityFrameworkCore;
using Shopping.Domain.Entities.Accounts;
using Shopping.Domain.Interfaces;
using Shopping.Persistence;
using Shopping.Persistence.Data;

namespace Shopping.Infrastructure.Repositories
{
    public class EmailVerificationRepository : BaseRepository<EmailVerification>, IEmailVerificationRepository
    {
        public EmailVerificationRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }

        public async Task<EmailVerification?> GetLatestActiveAsync(int userId)
        {
            var response = await _appDbContext.Set<EmailVerification>()
                .Where(m => m.UserId == userId
                         && m.UsedAt == null
                         && m.ExpiresAt > DateTime.UtcNow)
                .OrderByDescending(m => m.Id)
                .FirstOrDefaultAsync();

            return response;
        }
    }
}
