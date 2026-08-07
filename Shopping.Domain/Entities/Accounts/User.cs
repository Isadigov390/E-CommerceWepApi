using Shopping.Domain.Models;
using Shopping.Domain.Models.Common;

namespace Shopping.Domain.Entities.Accounts
{
    public class User : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        public bool EmailConfirmed { get; set; } = false;
        public ICollection<EmailVerification> EmailVerifications { get; set; } = new List<EmailVerification>();

        public ICollection<Review> Reviews { get; set; } = new List<Review>();

        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }

}
