using Microsoft.EntityFrameworkCore;
using Shopping.Domain.Entities.Accounts;
using Shopping.Domain.Interfaces;
using Shopping.Persistence;
using Shopping.Persistence.Data;

namespace Shopping.Infrastructure.Repositories
{
    public class RefreshTokenRepository : BaseRepository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }

        public async Task<RefreshToken?> GetByTokenHashAsync(string tokenHash)
        {
            return await _appDbContext.Set<RefreshToken>().Include(m => m.User).FirstOrDefaultAsync(m => m.TokenHash == tokenHash);
        }

        public async Task RotateAsync(RefreshToken currentToken, RefreshToken newToken)
        {
            _appDbContext.Set<RefreshToken>().Update(currentToken);
            await _appDbContext.Set<RefreshToken>().AddAsync(newToken);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task RevokeAsync(RefreshToken token)
        {
            _appDbContext.Set<RefreshToken>().Update(token);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task RevokeAllActiveForUserAsync(int userId)
        {
            var tokens = await _appDbContext.Set<RefreshToken>().Where(m => m.UserId == userId && m.RevokedAtUtc == null && m.ExpiresAtUtc > DateTime.UtcNow).ToListAsync();
            var revokedAtUtc = DateTime.UtcNow;

            foreach (var token in tokens)
            {
                token.RevokedAtUtc = revokedAtUtc;
            }

            await _appDbContext.SaveChangesAsync();
        }
    }
}
