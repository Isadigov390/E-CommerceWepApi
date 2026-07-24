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
    }
}
