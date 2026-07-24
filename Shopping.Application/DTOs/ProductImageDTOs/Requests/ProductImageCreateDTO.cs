
using Microsoft.AspNetCore.Http;
using Shopping.Application.Validations;
using System.ComponentModel.DataAnnotations;

namespace Shopping.Application.DTOs.ProductImageDTOs.Requests
{
    public class ProductImageCreateDTO
    {
        [AllowedExtensions(".jpg", ".jpeg", ".png")]
        [Required]
        public IFormFile? Image { get; set; }
    }
}
