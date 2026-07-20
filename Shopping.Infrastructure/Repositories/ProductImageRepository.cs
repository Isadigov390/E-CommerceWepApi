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
    }
}
