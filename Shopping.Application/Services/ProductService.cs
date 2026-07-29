using Microsoft.EntityFrameworkCore;
using Shopping.Application.DTOs.ProductDetailDTOs.Responses;
using Shopping.Application.DTOs.ProductDTOs.Requests;
using Shopping.Application.DTOs.ProductDTOs.Responses;
using Shopping.Application.DTOs.ProductImageDTOs.Responses;
using Shopping.Application.Handlers.Exceptions;
using Shopping.Application.ServiceInterfaces;
using Shopping.Domain.Enums;
using Shopping.Domain.Interfaces;
using Shopping.Domain.Models;

namespace Shopping.Application.Services
{

    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUrlService _urlService;
        private readonly IProductImageRepository _productImageRepository;
        public ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository , 
            IUrlService urlService, IProductImageRepository productImageRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _urlService = urlService;
            _productImageRepository = productImageRepository;
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

        public async Task<ProductWithParentAndChildrenDTO> GetProductByIdWithImages(int id)
        {
            var product = await _productRepository.GetByIdWithParentsAndChildren(id);
            if (product == null)
                throw new NotFoundException($"Product with id {id} not found.");

            var images = product.ProductImages
                .Where(i => i.IsMain)
                .Take(1)
                .Concat(product.ProductImages
                    .Where(i => !i.IsMain)
                    .OrderByDescending(i => i.CreatedAt)
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


            return new ProductWithParentAndChildrenDTO
            {
                Id = product.Id,
                Title = product.Title,
                Price = product.Price,
                Description = product.Description,
                Quantity = product.Quantity,
                Brand = product.Brand,
                CategoryId = product.CategoryId,
                CategoryName = product.Category.Name,
                SKU = product.ProductDetail?.SKU ?? string.Empty,
                Discount = product.ProductDetail?.Discount ?? 0,
                Warranty = product.ProductDetail?.Warranty ?? WarrantyType.NoWarranty,
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

        public async Task<ProductPagedResponseDTO> GetAllProductsWithPagination(
            ProductPaginationRequestDTO pagination)
        {
            // 1. BASE
            IQueryable<Product> query = _productRepository.GetAll(p => p.DeletedAt == null);

            // 2. SEARCH - only if the caller actually sent something
            if (!string.IsNullOrWhiteSpace(pagination.Search))
            {
                var term = pagination.Search.Trim();
                query = query.Where(p => p.Title.Contains(term));
            }

            // 3. TOTAL - after search, before paging
            var total = await query.CountAsync();

            // 4. SORT
            query = pagination.SortBy switch
            {
                ProductSortBy.PriceAsc =>
                    query.OrderBy(p => p.Price).ThenBy(p => p.Id),

                ProductSortBy.PriceDesc =>
                    query.OrderByDescending(p => p.Price).ThenBy(p => p.Id),

                ProductSortBy.TitleAsc =>
                    query.OrderBy(p => p.Title).ThenBy(p => p.Id),

                ProductSortBy.TitleDesc =>
                    query.OrderByDescending(p => p.Title).ThenBy(p => p.Id),

                ProductSortBy.RatingDesc =>
                    query.OrderByDescending(p => p.Reviews
                            .Where(r => r.DeletedAt == null)
                            .Average(r => (double?)r.Stars) ?? 0)
                         .ThenBy(p => p.Id),

                _ => query.OrderByDescending(p => p.CreatedAt).ThenBy(p => p.Id)
            };

            // 5. PAGE + load related data
            var products = await query
                .Skip(pagination.Skip)
                .Take(pagination.Limit)
                .Include(p => p.ProductImages.Where(i => i.IsMain && i.DeletedAt == null))
                .Include(p => p.Category)
                .ToListAsync();

            // 6. PROJECT in memory (BuildUrl can't translate to SQL)
            return new ProductPagedResponseDTO
            {
                Total = total,
                Skip = pagination.Skip,
                Limit = pagination.Limit,
                Products = products.Select(p => new ProductWithCategoryResponseDTO
                {
                    Id = p.Id,
                    Title = p.Title,
                    Price = p.Price,
                    Description = p.Description,
                    Quantity = p.Quantity,
                    Brand = p.Brand,
                    CategoryName = p.Category.Name,
                    CategoryId = p.CategoryId,
                    CreatedAt = p.CreatedAt,
                    LastModifiedAt = p.LastModifiedAt,
                    Images = p.ProductImages
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

        public async Task<ProductPagedResponseDTO> GetAllProductsWithPagination2(ProductPaginationRequestDTO pagination)
        {
            var query = _productRepository.GetAll(p=>p.DeletedAt == null);
            var total = query.Count();

            var products = await query.OrderByDescending(p=>p.CreatedAt).Skip(pagination.Skip).Take(pagination.Limit).Include(p=>p.ProductImages).Include(p=>p.Category)
                .ToListAsync();
            return new ProductPagedResponseDTO
            {
                Total = total,
                Skip = pagination.Skip,
                Limit = pagination.Limit,
                Products = products.Select(p => new ProductWithCategoryResponseDTO
                {
                    Id = p.Id,
                    Title = p.Title,
                    Price = p.Price,
                    Description = p.Description,
                    Quantity = p.Quantity,
                    Brand = p.Brand,
                    CategoryName= p.Category.Name,
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

        public async Task CreateProductWithImages(ProductWithImageIdsRequestDTO productWithImageIdsRequestDTO)
        {
            var createdProduct = await CreateAsync(new ProductRequestDTO
            {
                Title = productWithImageIdsRequestDTO.Title,
                Description = productWithImageIdsRequestDTO.Description,
                Brand = productWithImageIdsRequestDTO.Brand,
                CategoryId = productWithImageIdsRequestDTO.CategoryId,
                Price = productWithImageIdsRequestDTO.Price,
                Quantity = productWithImageIdsRequestDTO.Quantity,
            });
            
            await  _productImageRepository.AttachToProductAsync(createdProduct.Id, productWithImageIdsRequestDTO.ImageIds, productWithImageIdsRequestDTO.CoverImageId);

        }

        public async Task<ProductWithParentAndChildrenDTO> GetProductByIdWithAllImages(int id)
        {
            var product = await _productRepository.GetByIdWithParentsAndChildren(id);
            if (product == null)
                throw new NotFoundException($"Product with id {id} not found.");

            var images = product.ProductImages
                .Where(i => i.IsMain)
                .Take(1)
                .Concat(product.ProductImages
                    .Where(i => !i.IsMain)
                    .OrderByDescending(i => i.CreatedAt)
                    .Take(9))
                .Select(i => new ProductImageResponseDTO
                {
                    Id = i.Id,
                    FileName = i.FileName,
                    Url = _urlService.BuildUrl(i.FilePath),
                    IsMain = i.IsMain,
                    ProductId = i.ProductId
                })
                .ToList();


            return new ProductWithParentAndChildrenDTO
            {
                IsAvailable = product.Quantity > 0,
                Id = product.Id,
                Title = product.Title,
                Price = product.Price,
                Description = product.Description,
                Quantity = product.Quantity,
                Brand = product.Brand,
                CategoryId = product.CategoryId,
                CategoryName = product.Category.Name,
                SKU = product.ProductDetail?.SKU ?? string.Empty,
                Discount = product.ProductDetail?.Discount ?? 0,
                Warranty = product.ProductDetail?.Warranty ?? WarrantyType.NoWarranty,
                CreatedAt = product.CreatedAt,
                LastModifiedAt = product.LastModifiedAt,
                Images = images
               
            };
        }
        public async Task UpdateProductWithImages(int productId, ProductWithImageIdsRequestDTO dto)
        {
            // 1. product fields
            var product = await _productRepository.GetByIdAsync(productId);
            product.Title = dto.Title;
            product.Price = dto.Price;
            product.Description = dto.Description;
            product.Quantity = dto.Quantity;
            product.Brand = dto.Brand;
            product.CategoryId = dto.CategoryId;

            // 2. business rule: cover must be one of the product's images
            if (!dto.ImageIds.Contains(dto.CoverImageId))
                throw new ValidationException("Cover image must be one of the product's images.");

            // 3. reconcile images + set cover
            await _productImageRepository.SyncProductImagesAsync(productId, dto.ImageIds, dto.CoverImageId);

            await _productRepository.UpdateAsync(product);
        }

        public Task<IReadOnlyList<ProductResponseDTO>> GetAllProductsWithPagination()
        {
            throw new NotImplementedException();
        }
    }
}
