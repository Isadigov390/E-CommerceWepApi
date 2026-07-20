using Shopping.Domain.Models.Common;
using System.Linq.Expressions;

namespace Shopping.Domain.Entities.Interfaces
{
    public interface IBaseRepository<T> where T : BaseEntity
    {
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task DeleteAsync(int id);
        IQueryable<T> GetAll(Expression<Func<T, bool>> expression);
        Task<T> GetAsync(Expression<Func<T, bool>> expression);
        Task<bool> AnyAsync(Expression<Func<T, bool>> expression);
        Task AddRangeAsync(IEnumerable<T> entities);
    }
}
