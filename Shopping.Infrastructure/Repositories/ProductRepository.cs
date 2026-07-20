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

        public async Task<Product?> GetByIdWithImages(int id)
        {
            var product =  await _appDbContext.Set<Product>().Include(m=>m.ProductImages).FirstOrDefaultAsync(p=>p.Id == id); 
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
    }
}
