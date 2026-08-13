using Microsoft.EntityFrameworkCore;
using Shopping.Domain.Entities.Accounts;
using Shopping.Domain.Interfaces;
using Shopping.Persistence;
using Shopping.Persistence.Data;

namespace Shopping.Infrastructure.Repositories
{
    public class PasswordResetTokenRepository : BaseRepository<PasswordResetToken>, IPasswordResetTokenRepository
    {
        public PasswordResetTokenRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }

        public async Task<PasswordResetToken?> GetActiveByTokenHashAsync(string tokenHash)
        {
            var utcNow = DateTime.UtcNow;

            return await _appDbContext.Set<PasswordResetToken>().Include(x => x.User)
                .FirstOrDefaultAsync(x => x.TokenHash == tokenHash && x.UsedAtUtc == null && x.ExpiresAtUtc > utcNow && x.DeletedAt == null);
        }

        public async Task<PasswordResetToken?> GetLatestForUserAsync(int userId)
        {
            return await _appDbContext.Set<PasswordResetToken>()
                .Where(x => x.UserId == userId && x.DeletedAt == null).OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task CreateAndExpireOldTokensAsync(int userId, PasswordResetToken newToken)
        {
            var utcNow = DateTime.UtcNow;

            var activeTokens = await _appDbContext.Set<PasswordResetToken>()
                .Where(x => x.UserId == userId && x.UsedAtUtc == null && x.ExpiresAtUtc > utcNow && x.DeletedAt == null)
                .ToListAsync();

            foreach (var token in activeTokens)
            {
                token.ExpiresAtUtc = utcNow;
            }

            newToken.UserId = userId;

            await _appDbContext.Set<PasswordResetToken>().AddAsync(newToken);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task ResetPasswordAsync(PasswordResetToken resetToken)
        {
            _appDbContext.Set<User>().Update(resetToken.User);
            _appDbContext.Set<PasswordResetToken>().Update(resetToken);
            await _appDbContext.SaveChangesAsync();
        }
    }
}