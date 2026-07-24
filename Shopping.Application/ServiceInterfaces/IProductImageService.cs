using Shopping.Application.DTOs.ProductImageDTOs.Requests;
using Shopping.Application.DTOs.ProductImageDTOs.Responses;
using Shopping.Domain.Models;

namespace Shopping.Application.ServiceInterfaces
{
    public interface IProductImageService
    {
        public Task CreateByProductIdAsync(int id, ProductImagesCreateDTO productImageCreateDTO);
        public Task<ProductImageCreateResponseDTO> CreateOneAsync(ProductImageCreateDTO productImageCreateDTO);
        public Task<IReadOnlyList<ProductImageResponseDTO>> GetAllAsync();
        public Task<ProductImageResponseDTO> GetByIdAsync(int id); 
        public Task DeleteAsync(int id);
        public Task<ProductImage> UpdateAsync(int id, ProductImageUpdateRequestDTO productImageUpdateRequest);
        public Task<List<int>> CreateMany(ProductImagesCreateManyDTO productImagesCreateManyDTO);
    }
}
