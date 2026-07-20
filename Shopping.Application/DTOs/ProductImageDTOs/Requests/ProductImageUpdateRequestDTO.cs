using Microsoft.AspNetCore.Http;

namespace Shopping.Application.DTOs.ProductImageDTOs.Requests
{
    public class ProductImageUpdateRequestDTO
    {
        public int ProductId { get; set; }
        public List<IFormFile> NewImages { get; set; } = new();
        public List<int> DeletedImagesId { get; set; } = new();
        public int MainImageIndex { get; set; } = 0;
    }
}
