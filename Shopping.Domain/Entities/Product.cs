using Shopping.Domain.Models.Common;

namespace Shopping.Domain.Models
{
    public class Product : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
        //public string ImagePath { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string Brand { get; set; } = string.Empty;

        // FK
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        // navigation
        public ProductDetail ProductDetail { get; set; } = null!;
        public ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();

    }
}
