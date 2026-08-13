using Shopping.Domain.Models.Common;

namespace Shopping.Domain.Entities.Accounts
{
    public class PasswordResetToken : BaseEntity
    {
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public string TokenHash { get; set; } = string.Empty;
        public DateTime ExpiresAtUtc { get; set; }
        public DateTime? UsedAtUtc { get; set; }
    }
}
