using Shopping.Application.DTOs.ProductDetailDTOs.Requests;
using Shopping.Application.DTOs.ProductDetailDTOs.Responses;
using Shopping.Application.ServiceInterfaces;
using Shopping.Domain.Interfaces;
using Shopping.Domain.Models;

namespace Shopping.Application.Services
{
    public class ProductDetailService : IProductDetailService
    {
        private readonly IProductDetailRepository _productDetailRepository;
        public ProductDetailService(IProductDetailRepository productDetailRepository)
        {
            _productDetailRepository = productDetailRepository;
        }
        public async Task<ProductDetailResponseDTO> CreateAsync(ProductDetailRequestDTO productDetailRequestDTO)
        {
            ProductDetail productDetail = new ProductDetail()
            {
                ProductId = productDetailRequestDTO.ProductId,
                SKU = productDetailRequestDTO.SKU,
                Warranty = productDetailRequestDTO.Warranty,
                Discount = productDetailRequestDTO.Discount,
            };

            await _productDetailRepository.AddAsync(productDetail);

            ProductDetailResponseDTO pDetailResponse = new ProductDetailResponseDTO()
            {
                Id = productDetail.Id,
                ProductId = productDetail.ProductId,
                SKU = productDetail.SKU,
                Warranty = productDetail.Warranty,
                Discount = productDetail.Discount,
            };

            return pDetailResponse;
        }

        public async Task<IReadOnlyList<ProductDetailResponseDTO>> GetAllAsync()
        {
            var pDetails = await _productDetailRepository.GetAllAsync();
            List<ProductDetailResponseDTO> pDetailsResponse = new List<ProductDetailResponseDTO>(); 
            foreach (var p in pDetails) 
            {
                ProductDetailResponseDTO dTO = new ProductDetailResponseDTO()
                {
                    Id =p.Id,
                    ProductId = p.ProductId,
                    SKU = p.SKU,
                    Discount=p.Discount,
                    Warranty=p.Warranty,    
                };
                pDetailsResponse.Add(dTO);
            };
            return pDetailsResponse;
        }

        public async Task<ProductDetailResponseDTO> GetById(int id)
        {
            var pDetails = await _productDetailRepository.GetByIdAsync(id);
            ProductDetailResponseDTO pDetailsResponse = new ProductDetailResponseDTO()
            {
                Id = pDetails.Id,
                ProductId = pDetails.ProductId,
                SKU = pDetails.SKU,
                Discount= pDetails.Discount,
                Warranty= pDetails.Warranty,
            };
            return pDetailsResponse;
        }

    }
}

