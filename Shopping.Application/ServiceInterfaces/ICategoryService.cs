using Shopping.Application.DTOs.CategoryDTOs.Requests;
using Shopping.Application.DTOs.CategoryDTOs.Responses;
using Shopping.Domain.Models;

namespace Shopping.Application.ServiceInterfaces
{
    public interface ICategoryService
    {
        Task CreateAsync(CategoryRequestDTO categoryDTO);
        //Task<IReadOnlyList<Category>> GetAllAsync();
        Task<Category> GetByIdAsync(int id);
        Task DeleteAsync(int id);
        Task<CategoryResponseDTO> EditAsync(int id, CategoryRequestDTO categoryDTO);
        Task<IReadOnlyList<CategoryListResponseDTO>> GetAllAsync();
    }
}
