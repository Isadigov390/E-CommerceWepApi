using Shopping.Domain.Interfaces;
using Shopping.Domain.Models;
using Shopping.Persistence;
using Shopping.Persistence.Data;

namespace Shopping.Infrastructure.Repositories
{
    public class ProductDetailRepository : BaseRepository<ProductDetail>, IProductDetailRepository
    {
        public ProductDetailRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }
    }
}
