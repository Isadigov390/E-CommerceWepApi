using Microsoft.EntityFrameworkCore;
using Shopping.Domain.Interfaces;
using Shopping.Domain.Models;
using Shopping.Persistence;
using Shopping.Persistence.Data;

namespace Shopping.Infrastructure.Repositories
{
    public class ProductImageRepository : BaseRepository<ProductImage>, IProductImageRepository
    {
        public ProductImageRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }

        public async Task AttachToProductAsync(int productId, List<int> imageIds, int coverImageId)
        {
            var images = await _appDbContext.Set<ProductImage>()
                .Where(x => imageIds.Contains(x.Id))
                .ToListAsync();
            var mainImage = await _appDbContext.Set<ProductImage>().FirstOrDefaultAsync(m => m.Id ==coverImageId);
            if(mainImage is null)
            {
                throw new KeyNotFoundException("Cover Image Id is null");
            }
            foreach (var image in images)
                image.ProductId = productId;
            mainImage.IsMain = true;
            await _appDbContext.SaveChangesAsync();
        }

        public async Task SyncProductImagesAsync(int productId, List<int> desiredIds, int coverImageId)
        {
            var strategy = _appDbContext.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                using var tx = await _appDbContext.Database.BeginTransactionAsync();

                var current = await _appDbContext.Set<ProductImage>()
                    .Where(x => x.ProductId == productId && x.DeletedAt == null)
                    .ToListAsync();

                var desired = await _appDbContext.Set<ProductImage>()
                    .Where(x => desiredIds.Contains(x.Id) && x.DeletedAt == null)
                    .ToListAsync();

                if (desired.Count != desiredIds.Count)
                    throw new KeyNotFoundException("One or more image ids were not found.");

                // PHASE 1: clear mains + soft-delete removed
                foreach (var img in current)
                {
                    img.IsMain = false;
                    if (!desiredIds.Contains(img.Id))
                        _appDbContext.Set<ProductImage>().Remove(img);
                }
                await _appDbContext.SaveChangesAsync();

                // PHASE 2: attach + set single cover
                foreach (var img in desired)
                {
                    img.ProductId = productId;
                    img.IsMain = (img.Id == coverImageId);
                }
                await _appDbContext.SaveChangesAsync();

                await tx.CommitAsync();
            });
        }
    }
}
