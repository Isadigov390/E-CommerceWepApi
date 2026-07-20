using Shopping.Domain.Models.Common;

namespace Shopping.Domain.Models
{
    public class User : BaseEntity
    {
        public int PersonId { get; set; }
        public Person Person { get; set; } = null!;
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ProfileImagePath { get; set; } = string.Empty;
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
