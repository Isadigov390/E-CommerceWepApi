using Shopping.Domain.Models.Common;

namespace Shopping.Domain.Entities.Accounts
{
    public class EmailVerification : BaseEntity
    {
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public string CodeHash { get; set; } = string.Empty;

        public DateTime ExpiresAt { get; set; }
        public int AttemptCount { get; set; }

        public DateTime? UsedAt { get; set; }
    }
}
