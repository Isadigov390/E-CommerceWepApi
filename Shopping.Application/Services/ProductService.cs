using Microsoft.EntityFrameworkCore;
using Shopping.Application.DTOs.ProductDTOs.Requests;
using Shopping.Application.DTOs.ProductDTOs.Responses;
using Shopping.Application.DTOs.ProductImageDTOs.Responses;
using Shopping.Application.ServiceInterfaces;
using Shopping.Domain.Interfaces;
using Shopping.Domain.Models;

namespace Shopping.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUrlService _urlService;
        public ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository , IUrlService urlService)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _urlService = urlService;
        }
        public async Task<Product> CreateAsync(ProductRequestDTO productDTO)
        {
            
            Product product = new Product()
            {
                Title = productDTO.Title,
                Price = productDTO.Price,
                Description = productDTO.Description,
                Quantity = productDTO.Quantity,
                Brand = productDTO.Brand,
                CategoryId = productDTO.CategoryId,
                CreatedAt = DateTime.UtcNow,
            };
            await _productRepository.AddAsync(product);
            return product;

        }

        public Task DeleteAsync(int id)
        {
            return _productRepository.DeleteWithChildrenAsync(id);
        }

        public async Task<IReadOnlyList<Product>> GetAll()
        {
            return await _productRepository.GetAllAsync();
        }



        public async Task<IReadOnlyList<ProductResponseDTO>> GetAllProductsWithImages()
        {
            var products = await _productRepository.GetAllProductWithImages();

            return products.Select(p => new ProductResponseDTO
            {
                Id = p.Id,
                Title = p.Title,
                Price = p.Price,
                Description = p.Description,
                Quantity = p.Quantity,
                Brand = p.Brand,
                CategoryId = p.CategoryId,
                CreatedAt = p.CreatedAt,
                LastModifiedAt = p.LastModifiedAt,
                Images = p.ProductImages.Select(i => new ProductImageResponseDTO
                {
                    Id = i.Id,
                    FileName = i.FileName,
                    Url = _urlService.BuildUrl(i.FilePath),
                    IsMain = i.IsMain,
                    ProductId = i.ProductId,
                }).ToList()
            }).ToList();
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);

            return product;
        }

        public async Task<ProductResponseDTO> GetProductByIdWithImages(int id)
        {
            var product = await _productRepository.GetByIdWithImages(id);

            if (product == null)
                return null;

            var images = product.ProductImages
                .Where(i => i.IsMain)
                .Take(1)
                .Concat(product.ProductImages
                    .Where(i => !i.IsMain)
                    .OrderBy(i => i.CreatedAt)
                    .Take(3))
                .Select(i => new ProductImageResponseDTO
                {
                    Id = i.Id,
                    FileName = i.FileName,
                    Url = _urlService.BuildUrl(i.FilePath),
                    IsMain = i.IsMain,
                    ProductId = i.ProductId
                })
                .ToList();

            return new ProductResponseDTO
            {
                Id = product.Id,
                Title = product.Title,
                Price = product.Price,
                Description = product.Description,
                Quantity = product.Quantity,
                Brand = product.Brand,
                CategoryId = product.CategoryId,
                CreatedAt = product.CreatedAt,
                LastModifiedAt = product.LastModifiedAt,
                Images = images
            };
        }

        public async Task<ProductResponseDTO> UpdateAsync(int id, ProductRequestDTO productDTO)
        {
            var category = await _categoryRepository.GetByIdAsync(productDTO.CategoryId);

            var product = await _productRepository.GetByIdAsync(id);

            product.Title = productDTO.Title;
            product.Price = productDTO.Price;
            product.Description = productDTO.Description;
            product.Quantity = productDTO.Quantity;
            product.Brand = productDTO.Brand;
            product.CategoryId = productDTO.CategoryId;

            var updatedProduct = await _productRepository.UpdateAsync(product);

            return new ProductResponseDTO()
            {
                Title = updatedProduct.Title,
                Price = updatedProduct.Price,
                Description = updatedProduct.Description,
                Quantity = updatedProduct.Quantity,
                Brand = updatedProduct.Brand,
                CategoryId = updatedProduct.CategoryId,
                CreatedAt = updatedProduct.CreatedAt,
                LastModifiedAt = updatedProduct.LastModifiedAt
            };
        }

        public async Task<IReadOnlyList<ProductResponseDTO>> GetAllProductsWithPagination()
        {
            var products = await  _productRepository.GetAll(p => p.DeletedAt == null).Include(p => p.ProductImages).ToListAsync();

            return  products.Select(p => new ProductResponseDTO
            {
                Id = p.Id,
                Title = p.Title,
                Price = p.Price,
                Description = p.Description,
                Quantity = p.Quantity,
                Brand = p.Brand,
                CategoryId = p.CategoryId,
                CreatedAt = p.CreatedAt,
                LastModifiedAt = p.LastModifiedAt,
                Images = p.ProductImages.Where(i=>i.IsMain ==true)
                .Select(i => new ProductImageResponseDTO
                {
                    Id = i.Id,
                    FileName = i.FileName,
                    Url = _urlService.BuildUrl(i.FilePath),
                    IsMain = i.IsMain,
                    ProductId = i.ProductId,
                }).ToList()
            }).ToList();
        }

        public async Task<ProductResponseDTO> GetAsyncWithImages(int id)
        {
            var product = await _productRepository
                .GetAsync(p => p.Id == id && p.DeletedAt == null);
            var images = product.ProductImages.Where(i => i.IsMain == true).Take(1)
                .Concat(product.ProductImages.OrderBy(i => i.CreatedAt).Where(i => i.IsMain == false).Take(3))
                .Select(i => new ProductImageResponseDTO
                {
                    Id=i.Id,
                    FileName = i.FileName,
                    Url = _urlService.BuildUrl(i.FilePath),
                    IsMain = i.IsMain,
                    ProductId = i.ProductId,
                }).ToList();

            ProductResponseDTO respone = new ProductResponseDTO
            {
                Id = product.Id,
                Title = product.Title,
                Description = product.Description,
                Price = product.Price,
                Quantity = product.Quantity,
                Brand = product.Brand,
                CategoryId = product.CategoryId,
                CreatedAt = product.CreatedAt,
                LastModifiedAt = product.LastModifiedAt,
                Images = images
            };
            return respone;
        }

        public async Task<ProductPagedResponseDTO> GetAllProductsWithPagination(ProductPaginationRequestDTO pagination)
        {
            var query = _productRepository.GetAll(p=>p.DeletedAt == null);
            var total = query.Count();

            var products = await query.OrderBy(p=>p.CreatedAt).Skip(pagination.Skip).Take(pagination.Limit).Include(p=>p.ProductImages).ToListAsync();
            return new ProductPagedResponseDTO
            {
                Total = total,
                Skip = pagination.Skip,
                Limit = pagination.Limit,
                Products = products.Select(p => new ProductResponseDTO
                {
                    Id = p.Id,
                    Title = p.Title,
                    Price = p.Price,
                    Description = p.Description,
                    Quantity = p.Quantity,
                    Brand = p.Brand,
                    CategoryId = p.CategoryId,
                    CreatedAt = p.CreatedAt,
                    LastModifiedAt = p.LastModifiedAt,
                    Images = p.ProductImages.Where(i => i.IsMain == true)
                        .Select(i => new ProductImageResponseDTO
                        {
                            Id = i.Id,
                            FileName = i.FileName,
                            Url = _urlService.BuildUrl(i.FilePath),
                            IsMain = i.IsMain,
                            ProductId = i.ProductId,
                        }).ToList()
                }).ToList()
            };
        }
    }
}
