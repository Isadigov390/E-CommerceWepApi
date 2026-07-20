using Shopping.Domain.Models.Common;

namespace Shopping.Domain.Models
{
    public class Review : BaseEntity
    {
        public int Stars { get; set; }
        public string Comment { get; set; } = string.Empty;

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
