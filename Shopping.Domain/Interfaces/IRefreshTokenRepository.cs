using Shopping.Domain.Entities.Accounts;
using Shopping.Domain.Entities.Interfaces;

namespace Shopping.Domain.Interfaces
{
    public interface IRefreshTokenRepository : IBaseRepository<RefreshToken>
    {
        Task<RefreshToken?> GetByTokenHashAsync(string tokenHash);
        Task RotateAsync(RefreshToken currentToken, RefreshToken newToken);
        Task RevokeAsync(RefreshToken token);
        Task RevokeAllActiveForUserAsync(int userId);
    }
}