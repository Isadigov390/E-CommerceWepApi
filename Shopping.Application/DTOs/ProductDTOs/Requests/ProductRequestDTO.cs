using Shopping.Domain.Models;

namespace Shopping.Application.DTOs.ProductDTOs.Requests
{
    public class ProductRequestDTO
    {
        public string Title { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string Brand { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        //public List<ImageItemDTO> Images { get; set; } = new List<ImageItemDTO>();

    }
}
