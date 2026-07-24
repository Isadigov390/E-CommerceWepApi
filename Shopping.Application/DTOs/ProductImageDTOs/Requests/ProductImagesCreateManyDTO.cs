using Microsoft.AspNetCore.Http;

namespace Shopping.Application.DTOs.ProductImageDTOs.Requests
{
    public class ProductImagesCreateManyDTO
    {
        public List<IFormFile> Images { get; set; } = new List<IFormFile>();
    }
}
