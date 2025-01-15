using System.Linq.Expressions;

namespace Domain.Interfaces
{
    public interface IDbRepository<T>
        where T : class
    {
        Task<IEnumerable<T>> AllAsync();

        Task<T> GetByIdAsync(object id);

        Task AddAsync(T entity);

        Task<bool> AnyAsync();

        Task UpdateAsync(T entity);

        Task HardDeleteAsync(T entity);

        Task<int> SaveAsync(CancellationToken cancellationToken);

        Task<IEnumerable<T>> FilterAsync(Expression<Func<T, bool>> filter);

        Task<IEnumerable<TResult>> QueryAsync<TResult>(Func<IQueryable<T>, IQueryable<TResult>> query);

        void Dispose();
    }
}
