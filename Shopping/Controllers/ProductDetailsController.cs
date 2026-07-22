using Microsoft.AspNetCore.Mvc;
using Shopping.Application.DTOs.ProductDetailDTOs.Requests;
using Shopping.Application.ServiceInterfaces;

namespace Shopping.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductDetailsController : ControllerBase
    {
        private readonly IProductDetailService _productDetailService;
        public ProductDetailsController(IProductDetailService productDetailService)
        {
            _productDetailService = productDetailService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody]ProductDetailRequestDTO productDetailRequestDTO)
        {
            var pDetail = await _productDetailService.CreateAsync(productDetailRequestDTO);
            return Ok(pDetail);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var pDetail = await _productDetailService.GetById(id);
            return Ok(pDetail);
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var pDetails = await _productDetailService.GetAllAsync();
            return Ok(pDetails);
        }
    }
}
