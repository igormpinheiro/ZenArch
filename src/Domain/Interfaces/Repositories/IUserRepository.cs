using Domain.Entities;
using SharedKernel.Abstractions.Persistence;

namespace Domain.Interfaces.Repositories;

/// <summary>
/// Interface específica para o repositório de usuários
/// </summary>
public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}
