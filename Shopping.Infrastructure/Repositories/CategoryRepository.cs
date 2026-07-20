using Shopping.Domain.Interfaces;
using Shopping.Domain.Models;
using Shopping.Persistence;
using Shopping.Persistence.Data;

namespace Shopping.Infrastructure.Repositories
{
    public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }
    }
}
