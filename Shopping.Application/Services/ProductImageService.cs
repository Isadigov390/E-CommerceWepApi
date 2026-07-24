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
        public async Task CreateByProductIdAsync(int id, ProductImagesCreateDTO dto)
        {
            if (dto.Images == null || dto.Images.Count == 0)
                throw new ValidationException("At least one image is required.");

            if (dto.MainImageIndex < 0 || dto.MainImageIndex >= dto.Images.Count)
                throw new ValidationException("Main image index is invalid.");

            var mainAlreadyExists = await _productImageRepository
                .AnyAsync(x => x.ProductId == id && x.IsMain);
            if (mainAlreadyExists)  
                throw new ConflictException("This product already has a main image.");

            var images = new List<ProductImage>();
            for (int i = 0; i < dto.Images.Count; i++)
            {
                var file = dto.Images[i]; // takes first image file, then next next

                using var memoryStream = new MemoryStream();// instance of memory stream, but why? why not others like IFile(smthing else?)
                await file.CopyToAsync(memoryStream); //what does it do? saving copy of file to memory? 
                var bytes = memoryStream.ToArray(); // converts that image file which is in memory to bytes? why to bytes? 

                var extension = Path.GetExtension(file.FileName); // what this extension gives us? what file.FileName is like? car.png? 
                var savedPath = await _fileService.SaveAsync(bytes, extension); // here we use file service which has a method that accepts bytes and extension(is this .png?)

                images.Add(new ProductImage
                {
                    FilePath = savedPath,
                    FileName = file.FileName,
                    IsMain = i == dto.MainImageIndex,
                    ProductId = id
                }); //here we add to our created list and then send that list to db via repository method
            }

            await _productImageRepository.AddRangeAsync(images);
        }

        public async Task<List<int>> CreateMany(ProductImagesCreateManyDTO productImagesCreateManyDTO)
        {
            var requestImages = productImagesCreateManyDTO.Images;
            var images = new List<ProductImage>();

            foreach (var image in requestImages)
            {
                using var memoryStream = new MemoryStream();
                await image.CopyToAsync(memoryStream);
                var bytes = memoryStream.ToArray();

                var extension = Path.GetExtension(image.FileName);
                var savedPath = await _fileService.SaveAsync(bytes, extension);

                images.Add(new ProductImage
                {
                    FileName = image.FileName,
                    FilePath = savedPath
                });
            }

            await _productImageRepository.AddRangeAsync(images);   // SaveChanges fills each Id

            return images.Select(i => i.Id).ToList();              // read ids after save
        }

        public async Task<ProductImageCreateResponseDTO> CreateOneAsync(ProductImageCreateDTO productImageCreateDTO)
        {
            var file = productImageCreateDTO.Image;
            if (file.Length == 0 || file is null)
                throw new ValidationException("File is empty.");

            if (file.Length > 5 * 1024 * 1024)
                throw new ValidationException("File exceeds 5 MB.");

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            var bytes = memoryStream.ToArray();
            var extension = Path.GetExtension(file.FileName);
            var savedPath = await _fileService.SaveAsync(bytes, extension);

            ProductImage productImage = new ProductImage()
            {
                FileName = productImageCreateDTO.Image.FileName,
                FilePath = savedPath,
                IsMain = false,
            };
            await _productImageRepository.AddAsync(productImage);

            return new ProductImageCreateResponseDTO {Id = productImage.Id };

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
            ProductImagesCreateDTO productImageCreateDTO = new ProductImagesCreateDTO();
            productImageCreateDTO.MainImageIndex = productImageUpdateRequest.MainImageIndex;
            id = productImageUpdateRequest.ProductId;

            for (var i = 0; i<productImageUpdateRequest.DeletedImagesId.Count; i++)
            {
                await _productImageRepository.DeleteAsync(productImageUpdateRequest.DeletedImagesId[i]);
                productImageCreateDTO.Images.Add(productImageUpdateRequest.NewImages[i]);
            }
            await CreateByProductIdAsync(id, productImageCreateDTO);
            //_productImageRepository.UpdateAsync()


            return null;
        }
    }

}
