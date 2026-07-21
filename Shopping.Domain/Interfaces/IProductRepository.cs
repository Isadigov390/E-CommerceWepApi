using Shopping.Domain.Entities.Interfaces;
using Shopping.Domain.Models;
using System.Linq.Expressions;

namespace Shopping.Domain.Interfaces
{
    public interface IProductRepository : IBaseRepository<Product> 
    {
        public Task<IReadOnlyList<Product>> GetAllProductsWithMainImage();
        public Task<Product?> GetByIdWithImages(int id);
        public Task<IReadOnlyList<Product>> GetAllProductWithImages();
        public Task<Product> GetProductByIdWithImages(int id);
        public Task<Product> GetProductByIdWithImagesExpression(Expression<Func<Product, bool>> expression);
        public Task DeleteWithChildrenAsync(int id);
    }
}
