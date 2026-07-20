using Shopping.Application.DTOs.ProductImageDTOs.Responses;
using Shopping.Domain.Models;

namespace Shopping.Application.DTOs.ProductDTOs.Responses
{
    public class ProductResponseDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal Rating { get; set; }
        public string Description { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string Brand { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public List<ProductImageResponseDTO> Images { get; set; } = new();
        //public List<Review> Reviews { get; set; } = new();
    }
}
