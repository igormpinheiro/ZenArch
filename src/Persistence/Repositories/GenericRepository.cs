using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstractions.Domain;
using SharedKernel.Abstractions.Persistence;

namespace Persistence.Repositories;

/// <summary>
/// Implementação genérica do repositório
/// </summary>
/// <typeparam name="TEntity">Tipo da entidade</typeparam>
public class GenericRepository<TEntity> : IRepository<TEntity> where TEntity : class, IEntity
{
    protected readonly DbContext _dbContext;
    protected readonly DbSet<TEntity> _dbSet;

    public GenericRepository(DbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _dbSet = _dbContext.Set<TEntity>();
    }

    public virtual async Task<TEntity?> GetByIdAsync(object id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync([id], cancellationToken);
    }

    public virtual async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.ToListAsync(cancellationToken);
    }

    public virtual async Task<IReadOnlyList<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(predicate).ToListAsync(cancellationToken);
    }

    public virtual async Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet.SingleOrDefaultAsync(predicate, cancellationToken);
    }

    public virtual async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.CountAsync(cancellationToken);
    }

    public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet.CountAsync(predicate, cancellationToken);
    }

    public virtual async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(predicate, cancellationToken);
    }

    public virtual async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"DbContext instance in Repository: {_dbContext.GetHashCode()}");
        await _dbSet.AddAsync(entity, cancellationToken);
        Console.WriteLine($"Entity state after Add: {_dbContext.Entry(entity).State}");
        return entity;
    }

    public virtual async Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default)
    {
        var entitiesList = entities.ToList();
        await _dbSet.AddRangeAsync(entitiesList, cancellationToken);
        return entitiesList;
    }

    public virtual Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Entry(entity).State = EntityState.Modified;
        return Task.CompletedTask;
    }

    public virtual Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        _dbSet.UpdateRange(entities);
        return Task.CompletedTask;
    }

    public virtual Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Remove(entity);
        return Task.CompletedTask;
    }

    public virtual Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        _dbSet.RemoveRange(entities);
        return Task.CompletedTask;
    }

    public virtual async Task<IReadOnlyList<TEntity>> GetPaginatedAsync(
        int pageNumber, 
        int pageSize,
        Expression<Func<TEntity, bool>> predicate,
        string sortBy = "Id", 
        bool sortDescending = false,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsQueryable();
        query = sortDescending 
            ? query.OrderByDescending(e => EF.Property<object>(e, sortBy)) 
            : query.OrderBy(e => EF.Property<object>(e, sortBy));
        query = query.Where(predicate);
        return await query
            .Skip(pageNumber)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }
    
    public virtual async Task<IEnumerable<TEntity>> GetPaginatedAsync(
        int pageNumber, 
        int pageSize, 
        string sortBy = "Id", 
        bool sortDescending = false,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsQueryable();
        query = sortDescending 
                ? query.OrderByDescending(e => EF.Property<object>(e, sortBy)) 
                : query.OrderBy(e => EF.Property<object>(e, sortBy));
        return await query
            .Skip(pageNumber)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }
}
