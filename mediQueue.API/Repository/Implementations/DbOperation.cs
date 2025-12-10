using mediQueue.API.Context;
using mediQueue.API.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace mediQueue.API.Repository.Implementations
{
    public class DbOperation<T> : IDbOperation<T> where T : class
    {
        private readonly ApplicationDbContext dbContext;
        private readonly DbSet<T> dbSet;

        public DbOperation(ApplicationDbContext context)
        {
            dbContext = context;
            dbSet = dbContext.Set<T>();
        }


        public async Task AddAsync(T entity)
        {
            await dbSet.AddAsync(entity);
        }

        public void DeleteAsync(T entity)
        {
            dbSet.Remove(entity);
        }


        public async Task<ICollection<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {

            return await dbSet.Where(predicate).ToListAsync();
        }

        public async Task<ICollection<T>> GetAllWithIncludesAsync(params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = dbSet;

            foreach (var include in includes)
                query = query.Include(include);

            return await query.ToListAsync();
        }

        public async Task<T?> GetByIdWithIncludesAsync(Guid id, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = dbSet;

            foreach (var include in includes)
                query = query.Include(include);

            return await query.FirstOrDefaultAsync(x => EF.Property<Guid>(x, "Id") == id);
        }






        public async Task<ICollection<T>> GetAllAsync()
        {

            return await dbSet.ToListAsync();
        }


        public async Task<T?> GetByIdAsync(Guid id)
        {

            return await dbSet.FindAsync(id);
        }


        public async Task<int> SaveChangesAsync()
        {

            return await dbContext.SaveChangesAsync();
        }


        public void UpdateAsync(T entity)
        {
            dbSet.Update(entity);
        }
    }
}