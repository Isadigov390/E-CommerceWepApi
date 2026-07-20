using Microsoft.AspNetCore.Mvc;
using Shopping.Application.DTOs.CategoryDTOs.Requests;
using Shopping.Application.ServiceInterfaces;

namespace Shopping.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CategoryRequestDTO categoryDTO)
        {
            await _categoryService.CreateAsync(categoryDTO);
            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryService.GetAllAsync();
            return Ok(categories);
        }
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            return Ok(category);
        }
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _categoryService.DeleteAsync(id);
            return NoContent();
        }
        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Edit(int id, [FromBody] CategoryRequestDTO categoryDTO)
        {
            var updatedEntity = await _categoryService.EditAsync(id, categoryDTO);
            return Ok(updatedEntity);
        }
    }
}
