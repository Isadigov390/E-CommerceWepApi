
using Microsoft.AspNetCore.Http;

namespace Shopping.Application.DTOs.ProductImageDTOs.Requests
{
    public class ProductImageCreateDTO
    {
        public IFormFile Image { get; set; }
    }
}
