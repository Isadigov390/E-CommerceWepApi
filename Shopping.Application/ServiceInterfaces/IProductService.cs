using Shopping.Application.DTOs.ProductDTOs.Requests;
using Shopping.Application.DTOs.ProductDTOs.Responses;
using Shopping.Domain.Models;

namespace Shopping.Application.ServiceInterfaces
{
    public interface IProductService
    {
        Task<Product> CreateAsync(ProductRequestDTO productDTO);
        Task CreateProductWithImages(ProductWithImageIdsRequestDTO productWithImageIdsRequestDTO);
        Task<IReadOnlyList<Product>> GetAll();
        Task<Product> GetByIdAsync(int id);
        Task DeleteAsync(int id);
        Task<ProductResponseDTO> UpdateAsync(int id, ProductRequestDTO productDTO);
        Task<IReadOnlyList<ProductResponseDTO>> GetAllProductsWithImages();
        Task<ProductWithParentAndChildrenDTO> GetProductByIdWithImages(int id);
        Task<IReadOnlyList<ProductResponseDTO>> GetAllProductsWithPagination();
        Task<ProductResponseDTO> GetAsyncWithImages(int id);
        Task<ProductPagedResponseDTO> GetAllProductsWithPagination(ProductPaginationRequestDTO pagination);
    }
}
