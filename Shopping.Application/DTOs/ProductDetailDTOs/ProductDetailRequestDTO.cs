using Shopping.Domain.Enums;

namespace Shopping.Application.DTOs.ProductDetailDTOs
{
    public class ProductDetailRequestDTO
    {
        public string SKU { get; set; } = string.Empty;
        public int Discount { get; set; }
        
        public WarrantyType Warranty { get; set; }
        public int ProductId { get; set; }
    }
}
