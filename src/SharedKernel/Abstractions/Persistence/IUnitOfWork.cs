using SharedKernel.Abstractions.Domain;

namespace SharedKernel.Abstractions.Persistence;

//// <summary>
/// Interface para o Unit of Work
/// </summary>
public interface IUnitOfWork : IDisposable
{
    // Método para obter um repositório para uma entidade específica
    IRepository<TEntity> Repository<TEntity>() where TEntity : class, IEntity;
        
    // Método para salvar alterações
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        
    // Método para executar operações em uma transação
    Task<TResult> ExecuteTransactionAsync<TResult>(
        Func<Task<TResult>> operation, 
        CancellationToken cancellationToken = default);
            
    // Método para executar operações em uma transação (sem retorno)
    Task ExecuteTransactionAsync(
        Func<Task> operation, 
        CancellationToken cancellationToken = default);
}