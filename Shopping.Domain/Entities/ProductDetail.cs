using Shopping.Domain.Enums;
using Shopping.Domain.Models.Common;

namespace Shopping.Domain.Models
{
    public class ProductDetail : BaseEntity
    {
        public string SKU { get; set; } = string.Empty;
        public int Discount { get; set; }
        public WarrantyType Warranty { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
    }
}
