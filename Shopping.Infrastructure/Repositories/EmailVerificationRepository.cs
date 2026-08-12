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
                .Where(m => m.UserId == userId && m.UsedAt == null && m.ExpiresAt > DateTime.UtcNow && m.DeletedAt == null)
                .OrderByDescending(m => m.CreatedAt)
                .FirstOrDefaultAsync();

            return response;
        }

        public async Task<EmailVerification?> GetLatestAsync(int userId)
        {
            var response = await _appDbContext.Set<EmailVerification>()
                            .Where(m => m.UserId == userId && m.UsedAt == null && m.DeletedAt == null)
                            .OrderByDescending(m => m.CreatedAt)
                            .FirstOrDefaultAsync();

            return response;
        }

        public async Task<EmailVerification> ResendVerificationEmailAsync(int userId, EmailVerification newVerification)
        {
            var utcNow = DateTime.UtcNow;

            var activeVerifications = await _appDbContext.Set<EmailVerification>()
                .Where(ev => ev.UserId == userId && ev.UsedAt == null && ev.ExpiresAt > utcNow && ev.DeletedAt == null)
                .ToListAsync();

            foreach (var verification in activeVerifications)
            {
                verification.ExpiresAt = utcNow;
            }

            newVerification.UserId = userId;

            await _appDbContext.Set<EmailVerification>().AddAsync(newVerification);
            await _appDbContext.SaveChangesAsync();

            return newVerification;
        }
    }
}
