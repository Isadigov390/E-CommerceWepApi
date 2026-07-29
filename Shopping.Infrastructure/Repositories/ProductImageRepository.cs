using Microsoft.EntityFrameworkCore;
using Shopping.Domain.Interfaces;
using Shopping.Domain.Models;
using Shopping.Persistence;
using Shopping.Persistence.Data;
using System.ComponentModel.DataAnnotations;

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
            // input cleanup: duplicates would break the count check below
            var ids = desiredIds.Distinct().ToList();

            // retry policy from EnableRetryOnFailure in Program.cs.
            // required because we open an explicit transaction: EF refuses to
            // retry a half-applied transaction, so the whole block is the retry unit.
            var strategy = _appDbContext.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                // both SaveChanges below succeed together or not at all.
                // no CommitAsync => dispose rolls everything back.
                using var tx = await _appDbContext.Database.BeginTransactionAsync();

                // every live image currently attached to this product
                var current = await _appDbContext.Set<ProductImage>()
                    .Where(x => x.ProductId == productId && x.DeletedAt == null)
                    .ToListAsync();

                // requested images, limited to ones this product may claim:
                // already mine, or unattached. blocks stealing another product's image.
                var desired = await _appDbContext.Set<ProductImage>()
                    .Where(x => ids.Contains(x.Id)
                             && x.DeletedAt == null
                             && (x.ProductId == productId || x.ProductId == null))
                    .ToListAsync();

                // validate everything before mutating anything
                if (desired.Count != ids.Count)
                    throw new KeyNotFoundException("One or more image ids were not found.");

                if (!ids.Contains(coverImageId))
                    throw new ArgumentException("Cover image must be one of the submitted image ids.");

                // PHASE 1: clear all mains, soft-delete the ones dropped by the caller.
                // Remove() is intercepted by the SaveChangesAsync override in AppDbContext
                // and turned into DeletedAt = UtcNow, so the row survives.
                foreach (var img in current)
                {
                    img.IsMain = false;
                    if (!ids.Contains(img.Id))
                        _appDbContext.Set<ProductImage>().Remove(img);
                }

                // must be its own round-trip: the unique filtered index on
                // (ProductId, IsMain) rejects two live mains, so the old cover
                // has to be cleared before the new one is set.
                await _appDbContext.SaveChangesAsync();

                // PHASE 2: attach the kept images and mark exactly one cover
                foreach (var img in desired)
                {
                    img.ProductId = productId;
                    img.IsMain = (img.Id == coverImageId);
                }
                await _appDbContext.SaveChangesAsync();

                await tx.CommitAsync();
            });
        }


        public async Task SyncProductImagesAsyncc(int productId, List<int> desiredIds, int coverImageId)
        {
            var previousImagesInProduct = await _appDbContext.Set<ProductImage>().Where(x=>x.ProductId == productId).ToListAsync();
            _appDbContext.RemoveRange(previousImagesInProduct);

        }
    }
}
