using Microsoft.AspNetCore.Mvc;
using Shopping.Application.DTOs.ProductDetailDTOs;

namespace Shopping.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductDetailsController : ControllerBase
    {
        [HttpPost]
        public IActionResult Create(ProductDetailRequestDTO productDetailRequestDTO)
        {
            return NoContent();
        }
    }
}
