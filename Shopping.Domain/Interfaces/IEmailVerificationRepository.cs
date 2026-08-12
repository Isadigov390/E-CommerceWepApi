using Shopping.Domain.Entities.Accounts;
using Shopping.Domain.Entities.Interfaces;

namespace Shopping.Domain.Interfaces
{
    public interface IEmailVerificationRepository : IBaseRepository<EmailVerification>
    {
        Task<EmailVerification?> GetLatestActiveAsync(int userId);
        Task<EmailVerification?> GetLatestAsync(int userId);
        Task<EmailVerification> ResendVerificationEmailAsync(int userId, EmailVerification newEmailVerification);

    }
}
