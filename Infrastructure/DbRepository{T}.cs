using Domain.Interfaces;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

public class DbRepository<T> : IDbRepository<T> where T : class
{
    private DbSet<T> DbSet { get; }
    private IApplicationDbContext Context { get; }

    public DbRepository(ApplicationDbContext context)
    {
        Context = context ?? throw new ArgumentException("An instance of DbContext is required to use this repository.", nameof(context));
        DbSet = Context.Set<T>();
    }

    public async Task<T> GetByIdAsync(object id)
    {
        return await DbSet.FindAsync(id);
    }

    public async Task<bool> AnyAsync()
    {
        return await DbSet.AnyAsync();
    }

    public async Task<int> SaveAsync(CancellationToken cancellationToken)
    {
        return await Context.SaveChangesAsync(cancellationToken);
    }

    public async Task AddAsync(T entity)
    {
        await DbSet.AddAsync(entity);
    }
    public async Task UpdateAsync(T entity)
    {
        var entry = Context.Entry(entity);
        if (entry.State == EntityState.Detached)
        {
            await DbSet.AddAsync(entity);
        }
        entry.State = EntityState.Modified;
    }

    public async Task HardDeleteAsync(T entity)
    {
        DbSet.Remove(entity);
        await Task.CompletedTask;
    }

    public async Task<IEnumerable<T>> FilterAsync(Expression<Func<T, bool>> filter)
    {
        return await DbSet.Where(filter).ToListAsync();
    }

    public void Dispose()
    {
        Context.Dispose();
    }

    public async Task<IEnumerable<T>> AllAsync()
    {
        return await DbSet.ToListAsync();
    }

    public async Task<IEnumerable<TResult>> QueryAsync<TResult>(Func<IQueryable<T>, IQueryable<TResult>> query)
    {
        if (query == null)
            throw new ArgumentNullException(nameof(query));

        return await query(DbSet).ToListAsync();
    }
}
