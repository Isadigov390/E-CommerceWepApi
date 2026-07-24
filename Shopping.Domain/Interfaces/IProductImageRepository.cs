using Shopping.Domain.Entities.Interfaces;
using Shopping.Domain.Models;

namespace Shopping.Domain.Interfaces
{
    public interface IProductImageRepository : IBaseRepository<ProductImage>
    {
        Task AttachToProductAsync(int productId, List<int> imageIds, int coverImageId);
        public Task SyncProductImagesAsync(int productId, List<int> desiredIds, int coverImageId);

    }
}
