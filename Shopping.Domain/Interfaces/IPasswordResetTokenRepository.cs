using Shopping.Domain.Entities.Accounts;
using Shopping.Domain.Entities.Interfaces;

namespace Shopping.Domain.Interfaces
{
    public interface IPasswordResetTokenRepository : IBaseRepository<PasswordResetToken>
    {
        Task<PasswordResetToken?> GetActiveByTokenHashAsync(string tokenHash);
        Task<PasswordResetToken?> GetLatestForUserAsync(int userId);
        Task CreateAndExpireOldTokensAsync(int userId, PasswordResetToken newToken);
        Task ResetPasswordAsync(PasswordResetToken resetToken);
    }
}