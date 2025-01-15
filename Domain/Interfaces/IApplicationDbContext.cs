using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Domain.Interfaces
{
    public interface IApplicationDbContext
    {
        DbContext DbContext { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);

        void Dispose();

        EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;

        DbSet<T> Set<T>() where T : class;
    }
}
