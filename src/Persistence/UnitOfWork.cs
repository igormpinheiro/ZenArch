using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using Persistence.Repositories;
using SharedKernel.Abstractions.Domain;
using SharedKernel.Abstractions.Persistence;

namespace Persistence;

/// <summary>
/// Implementação do Unit of Work adaptada para usar a estratégia de execução do EF Core
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext  _dbContext;
    private readonly Dictionary<Type, object> _repositories;
    private bool _disposed;

    public UnitOfWork(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _repositories = new Dictionary<Type, object>();
    }

    public IRepository<TEntity> Repository<TEntity>() where TEntity : class, IEntity
    {
        var type = typeof(TEntity);

        if (!_repositories.TryGetValue(type, out object? value))
        {
            value = new GenericRepository<TEntity>(_dbContext);
            _repositories[type] = value;
        }

        return (IRepository<TEntity>)value;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<TResult> ExecuteTransactionAsync<TResult>(
        Func<Task<TResult>> operation,
        CancellationToken cancellationToken = default)
    {
        // Criar estratégia de execução usando o contexto
        var strategy = _dbContext.Database.CreateExecutionStrategy();

        // Executar a operação usando a estratégia de execução
        return await strategy.ExecuteAsync(async () =>
        {
            // Iniciar a transação
            using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                // Executar a operação
                var result = await operation();

                // Salvar alterações
                await _dbContext.SaveChangesAsync(cancellationToken);

                // Commit da transação
                await transaction.CommitAsync(cancellationToken);

                return result;
            }
            catch
            {
                // Rollback em caso de exceção
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        });
    }

    public async Task ExecuteTransactionAsync(
        Func<Task> operation,
        CancellationToken cancellationToken = default)
    {
        // Implementação sem retorno que chama a versão com retorno
        await ExecuteTransactionAsync(async () =>
        {
            await operation();
            return true; // Valor fictício que será descartado
        }, cancellationToken);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _dbContext.Dispose();
        }

        _disposed = true;
    }
}
