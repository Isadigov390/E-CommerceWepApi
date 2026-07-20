using Shopping.Application.DTOs.ProductImageDTOs.Requests;
using Shopping.Application.DTOs.ProductImageDTOs.Responses;
using Shopping.Domain.Models;

namespace Shopping.Application.ServiceInterfaces
{
    public interface IProductImageService
    {
        public Task CreateAsync(ProductImageCreateDTO productImageCreateDTO);
        public Task<IReadOnlyList<ProductImageResponseDTO>> GetAllAsync();
        public Task<ProductImageResponseDTO> GetByIdAsync(int id); 
        public Task DeleteAsync(int id);
        public Task<ProductImage> UpdateAsync(int id, ProductImageUpdateRequestDTO productImageUpdateRequest);
    }
}
