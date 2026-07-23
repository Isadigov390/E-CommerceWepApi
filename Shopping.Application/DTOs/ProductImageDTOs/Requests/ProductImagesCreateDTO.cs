using Microsoft.AspNetCore.Http;

namespace Shopping.Application.DTOs.ProductImageDTOs.Requests
{
    public class ProductImagesCreateDTO
    {
        public List<IFormFile> Images { get; set; } = new();
        public int MainImageIndex { get; set; } = 0;
    }
}
