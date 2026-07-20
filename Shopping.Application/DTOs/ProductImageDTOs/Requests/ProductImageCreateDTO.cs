using Microsoft.AspNetCore.Http;

namespace Shopping.Application.DTOs.ProductImageDTOs.Requests
{
    public class ProductImageCreateDTO
    {
        public int ProductId { get; set; }
        public List<IFormFile> Images { get; set; } = new();
        public int MainImageIndex { get; set; } = 0;
    }
}
