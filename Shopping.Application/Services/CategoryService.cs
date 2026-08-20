using Shopping.Application.DTOs.CategoryDTOs.Requests;
using Shopping.Application.DTOs.CategoryDTOs.Responses;
using Shopping.Application.ServiceInterfaces;
using Shopping.Domain.Interfaces;
using Shopping.Domain.Models;

namespace Shopping.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public async Task CreateAsync(CategoryRequestDTO categoryDTO)
        {
            var category = new Category()
            {
                Name = categoryDTO.Name,
              //  CreatedAt = DateTime.Now,
            };
            await _categoryRepository.AddAsync(category);
        }

        public async Task<IReadOnlyList<CategoryListResponseDTO>> GetAllAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();

            return categories
                .Where(category => category.DeletedAt == null)
                .OrderBy(category => category.Name)
                .Select(category => new CategoryListResponseDTO
                {
                    Id = category.Id,
                    Name = category.Name
                })
                .ToList();
        }

        public async Task<Category> GetByIdAsync(int id)
        {
            var entity = await _categoryRepository.GetByIdAsync(id);
            if (entity is null)
            {
                throw new KeyNotFoundException();
            }   
            return entity;
        }

        public async Task DeleteAsync(int id)
        {
            await _categoryRepository.DeleteAsync(id);
        }

        public async Task<CategoryResponseDTO> EditAsync(int id, CategoryRequestDTO categoryDTO)
        {
            var category = await _categoryRepository.GetByIdAsync(id);

            category.Name = categoryDTO.Name;

            var updatedCategory = await _categoryRepository.UpdateAsync(category);

            return new CategoryResponseDTO()
            {
                Id = updatedCategory.Id,
                Name = updatedCategory.Name,
                CreatedAt = updatedCategory.CreatedAt,
                LastModifiedAt = updatedCategory.LastModifiedAt,
            };
        }
    }
}
