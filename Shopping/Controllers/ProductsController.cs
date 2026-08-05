using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shopping.Application.DTOs;
using Shopping.Application.DTOs.ProductDTOs.Requests;
using Shopping.Application.ServiceInterfaces;

namespace Shopping.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetAll();
            return Ok(products);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductRequestDTO productDTO)
        {
            var product = await _productService.CreateAsync(productDTO);
            return Ok(product);
        }
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById([FromRoute]int id)
        {
            var product = await _productService.GetByIdAsync(id);
            return Ok(product);
        }
        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Update([FromRoute]int id, [FromBody] ProductRequestDTO productDTO)
        {
            var product = await _productService.UpdateAsync(id, productDTO);
            return Ok(product);
        }
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete([FromRoute]int id)
        {
            await _productService.DeleteAsync(id);
            return NoContent();
        }
        //[HttpGet("with-images")]
        //public async Task<IActionResult> GetAllProductsWithImages()
        //{
        //    var products = await _productService.GetAllProductsWithPagination();
        //    return Ok(products);
        //}
        [Authorize]
        [HttpGet("{id}/with-images")]
        public async Task<IActionResult> GetProductsByIdWithImages([FromRoute] int id)
        {
            var product = await _productService.GetProductByIdWithImages(id);
            return Ok(product);
        }
        [HttpGet("{id}/with-all-images")]
        public async Task<IActionResult> GetProductsByIdWithAllImages([FromRoute] int id)
        {
            var product = await _productService.GetProductByIdWithAllImages(id);
            return Ok(product);
        }
        [HttpGet("with-images")]
        public async Task<IActionResult> GetProductsPaged([FromQuery] ProductPaginationRequestDTO pagination)
        {
            var products = await _productService.GetAllProductsWithPagination(pagination);
            return Ok(products);
        }

        [HttpPost("with-images")]
        public async Task<IActionResult> CreateProductWithImageIds(ProductWithImageIdsRequestDTO request)
        {
            await _productService.CreateProductWithImages(request);
            return Ok();
        }

        [HttpPut("{id}/with-images")]
        public async Task<IActionResult> UpdateProductWithImages([FromRoute] int id, ProductWithImageIdsRequestDTO request)
        {
            await _productService.UpdateProductWithImages(id, request);
            return NoContent();
        }
    }
}
