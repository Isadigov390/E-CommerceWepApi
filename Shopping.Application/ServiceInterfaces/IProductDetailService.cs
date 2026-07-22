using Shopping.Application.DTOs.ProductDetailDTOs.Requests;
using Shopping.Application.DTOs.ProductDetailDTOs.Responses;

namespace Shopping.Application.ServiceInterfaces
{
    public interface IProductDetailService
    {
        public Task<ProductDetailResponseDTO> CreateAsync(ProductDetailRequestDTO productDetailRequestDTO);
        public Task<ProductDetailResponseDTO> GetById(int id);
        public Task<IReadOnlyList<ProductDetailResponseDTO>> GetAllAsync();
    }
}
