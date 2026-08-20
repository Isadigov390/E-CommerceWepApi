using Shopping.Application.DTOs.ProductImageDTOs.Responses;
using Shopping.Application.DTOs.ReviewDTOs.Responses;
using Shopping.Domain.Enums;

namespace Shopping.Application.DTOs.ProductDTOs.Responses
{
    public class ProductWithParentAndChildrenDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal Rating { get; set; }
        public int ReviewCount { get; set; }
        public string Description { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string Brand { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public DateTime? CreatedAt { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public string SKU { get; set; } = string.Empty;
        public int Discount { get; set; }
        public WarrantyType Warranty { get; set; }
        public bool IsAvailable { get; set; }
        public List<ProductImageResponseDTO> Images { get; set; } = new();
        public List<ReviewResponseDTO> Reviews { get; set; } = new();
    }
}
