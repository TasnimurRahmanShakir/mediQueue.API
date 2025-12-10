using System.Linq.Expressions;

namespace mediQueue.API.Repository.Interfaces
{
    public interface IDbOperation<T> where T : class
    {
        Task<ICollection<T>> GetAllAsync();
        Task<ICollection<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<T?> GetByIdAsync(Guid id);

        Task<ICollection<T>> GetAllWithIncludesAsync(params Expression<Func<T, object>>[] includes);
        Task<T?> GetByIdWithIncludesAsync(Guid id, params Expression<Func<T, object>>[] includes);

        Task AddAsync(T entity);
        void UpdateAsync(T entity);
        void DeleteAsync(T entity);



        Task<int> SaveChangesAsync();
    }
}
