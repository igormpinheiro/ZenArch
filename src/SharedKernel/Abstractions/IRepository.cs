using System.Linq.Expressions;

namespace SharedKernel.Abstractions;

// <summary>
/// Interface genérica para operações de repositório
/// </summary>
/// <typeparam name="TEntity">Tipo da entidade</typeparam>
public interface IRepository<TEntity> where TEntity : class, IEntity
{
    // Métodos de consulta
    Task<TEntity?> GetByIdAsync(object id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);

    Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);

    Task<int> CountAsync(CancellationToken cancellationToken = default);
    Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    // Métodos de modificação
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    // Métodos avançados de consulta
    Task<IReadOnlyList<TEntity>> GetPagedResponseAsync(int pageNumber, int pageSize,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TEntity>> GetPagedResponseAsync(int pageNumber, int pageSize,
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);
}
