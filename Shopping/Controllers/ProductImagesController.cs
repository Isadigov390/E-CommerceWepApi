using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shopping.Application.DTOs.ProductImageDTOs.Requests;
using Shopping.Application.ServiceInterfaces;

namespace Shopping.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductImagesController : ControllerBase
    {
        private readonly IProductImageService _productImageService;
        public ProductImagesController(IProductImageService productImageService)
        {
            _productImageService = productImageService;
        }
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromForm] ProductImageCreateDTO productImageCreateDTO)
        {
            await _productImageService.CreateAsync(productImageCreateDTO);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var images = await _productImageService.GetAllAsync();
            return  Ok(images);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var image = await _productImageService.GetByIdAsync(id);
            return Ok(image);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _productImageService.DeleteAsync(id);
            return NoContent();
        }
        //[HttpPut("{id}")]
        //public async Task<IActionResult> Update(int id, [FromForm] ProductImageUpdateRequestDTO productImageUpdateRequestDTO)
        //{
        //    return NoContent();
        //}
    }
}
