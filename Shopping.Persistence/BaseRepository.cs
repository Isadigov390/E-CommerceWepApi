using Microsoft.EntityFrameworkCore;
using Shopping.Domain.Entities.Interfaces;
using Shopping.Domain.Models.Common;
using Shopping.Persistence.Data;
using System.Linq.Expressions;

namespace Shopping.Persistence
{
    public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
    {
        protected readonly AppDbContext _appDbContext;
        public BaseRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task AddAsync(T entity)
        {
            await _appDbContext.Set<T>().AddAsync(entity);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
           var entity =  await _appDbContext.Set<T>().FindAsync(id);
            if (entity == null)
            {
                throw new KeyNotFoundException($"Entity of type {typeof(T).Name} with id {id} not found.");
            }
            _appDbContext.Set<T>().Remove(entity);
            await _appDbContext.SaveChangesAsync();
        }



        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await _appDbContext.Set<T>().ToListAsync();
        }


        public async Task<T> GetByIdAsync(int id)
        {
            var entity =  await _appDbContext.Set<T>().FindAsync(id);
            if (entity == null)
            {
                throw new KeyNotFoundException($"Entity of type {typeof(T).Name} with id {id} not found.");
            }
            return entity;
        }

        public async Task<T> UpdateAsync(T entity)
        {
            _appDbContext.Set<T>().Update(entity);
            await _appDbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> expression)
        {
            var entity = await _appDbContext.Set<T>().FirstOrDefaultAsync(expression);
            if (entity == null)
            {
                throw new KeyNotFoundException($"{typeof(T).Name} with the given ID is not found");
            }
            return entity;
        }
        public IQueryable<T> GetAll(Expression<Func<T, bool>> expression)
        {
            var query = _appDbContext.Set<T>().Where(expression);
            return query;
        }
    }
}
//ef interceptors on saveChanges for auditableEntities