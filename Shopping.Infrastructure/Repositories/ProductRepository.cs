using Microsoft.EntityFrameworkCore;
using Shopping.Domain.Interfaces;
using Shopping.Domain.Models;
using Shopping.Persistence;
using Shopping.Persistence.Data;
using System.Linq.Expressions;

namespace Shopping.Infrastructure.Repositories
{


    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }

        public async Task<IReadOnlyList<Product>> GetAllProductWithImages()
        {
            return await _appDbContext.Set<Product>().Include(m=>m.ProductImages).ToListAsync();
        }

        public async Task<IReadOnlyList<Product>> GetAllProductsWithMainImage()
        {
           return await _appDbContext.Set<Product>().Include(m=>m.ProductImages.Where(i=>i.IsMain)).ToListAsync();
        }

        public async Task<Product?> GetByIdWithParentsAndChildren(int id)
        {
            var product =  await _appDbContext.Set<Product>().Include(m=>m.ProductImages).Include(m=>m.ProductDetail)
                .Include(m=>m.Category).Include(r=>r.Reviews.Where(rD=>rD.DeletedAt == null)).ThenInclude(review => review.User)
                .Where(r => r.DeletedAt == null)
                .FirstOrDefaultAsync(p=>p.Id == id); 
            if (product == null)
            {
                 throw new KeyNotFoundException();
            }
            return product; 
        }

        public async Task<Product> GetProductByIdWithImages(int id)
        {
            var product = await _appDbContext.Set<Product>().Include(p => p.ProductImages).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                throw new KeyNotFoundException();
            }

           return product;
        }

        public Task<Product> GetProductByIdWithImagesExpression(Expression<Func<Product, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteWithChildrenAsync(int id)
        {
           var productWithImages = await _appDbContext.Set<Product>()
                .Include(p=>p.ProductImages).Include(p=>p.Reviews)
                .FirstOrDefaultAsync(p=>p.Id == id);
            if(productWithImages is null)
                throw new KeyNotFoundException();
             _appDbContext.Remove(productWithImages);
            await _appDbContext.SaveChangesAsync();
        }
    }
}
