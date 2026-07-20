using Shopping.Application.DTOs.ProductImageDTOs.Requests;
using Shopping.Application.DTOs.ProductImageDTOs.Responses;
using Shopping.Application.Handlers.Exceptions;
using Shopping.Application.ServiceInterfaces;
using Shopping.Domain.Interfaces;
using Shopping.Domain.Models;

namespace Shopping.Application.Services
{
    public class ProductImageService : IProductImageService
    {
        private readonly IProductImageRepository _productImageRepository;
        private readonly IFileService _fileService;
        private readonly IUrlService _urlService;

        public ProductImageService(IProductImageRepository productImageRepository, IFileService fileService, IUrlService urlService)
        {
            _productImageRepository = productImageRepository;
            _fileService = fileService;
            _urlService = urlService;
        }
        public async Task CreateAsync(ProductImageCreateDTO dto)
        {
            if (dto.Images == null || dto.Images.Count == 0)
                throw new ValidationException("At least one image is required.");

            if (dto.MainImageIndex < 0 || dto.MainImageIndex >= dto.Images.Count)
                throw new ValidationException("Main image index is invalid.");

            var mainAlreadyExists = await _productImageRepository
                .AnyAsync(x => x.ProductId == dto.ProductId && x.IsMain);
            if (mainAlreadyExists)
                throw new ConflictException("This product already has a main image.");

            var images = new List<ProductImage>();
            for (int i = 0; i < dto.Images.Count; i++)
            {
                var file = dto.Images[i];

                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);
                var bytes = memoryStream.ToArray();

                var extension = Path.GetExtension(file.FileName);
                var savedPath = await _fileService.SaveAsync(bytes, extension);

                images.Add(new ProductImage
                {
                    FilePath = savedPath,
                    FileName = file.FileName,
                    IsMain = i == dto.MainImageIndex,
                    ProductId = dto.ProductId
                });
            }

            await _productImageRepository.AddRangeAsync(images);
        }

        public async Task DeleteAsync(int id)
        {
            var image = await _productImageRepository.GetByIdAsync(id); 
            await _fileService.DeleteAsync(image.FilePath);            
            await _productImageRepository.DeleteAsync(id);            
        }

        public async Task<IReadOnlyList<ProductImageResponseDTO>> GetAllAsync()
        {
            var entities = await _productImageRepository.GetAllAsync();
            return entities.Select(i => new ProductImageResponseDTO
            {
                Id = i.Id,
                FileName = i.FileName,
                IsMain = i.IsMain,
                ProductId = i.ProductId,
                Url = _urlService.BuildUrl(i.FilePath)
            }).ToList();
        }

        public async Task<ProductImageResponseDTO> GetByIdAsync(int id)
        {
            var i = await _productImageRepository.GetByIdAsync(id);
            return new ProductImageResponseDTO
            {
                Id = i.Id,
                FileName = i.FileName,
                IsMain = i.IsMain,
                ProductId = i.ProductId,
                Url = _urlService.BuildUrl(i.FilePath)
            };
        }

        public async Task<ProductImage> UpdateAsync(int id, ProductImageUpdateRequestDTO productImageUpdateRequest)
        {
            var productImage = await _productImageRepository.GetByIdAsync(id);
            ProductImageCreateDTO productImageCreateDTO = new ProductImageCreateDTO();
            productImageCreateDTO.MainImageIndex = productImageUpdateRequest.MainImageIndex;
            productImageCreateDTO.ProductId = productImageUpdateRequest.ProductId;

            for (var i = 0; i<productImageUpdateRequest.DeletedImagesId.Count; i++)
            {
                await _productImageRepository.DeleteAsync(productImageUpdateRequest.DeletedImagesId[i]);
                productImageCreateDTO.Images.Add(productImageUpdateRequest.NewImages[i]);
            }
            await CreateAsync(productImageCreateDTO);
            //_productImageRepository.UpdateAsync()


            return null;
        }
    }

}
