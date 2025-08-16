using Domain.Entities;
using Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories;

/// <summary>
/// Implementação específica para o repositório de usuários
/// </summary>
public class UserRepository(ApplicationDbContext dbContext) : GenericRepository<User>(dbContext), IUserRepository
{
    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public override async Task<IEnumerable<User>> GetPaginatedAsync(
        int pageNumber, 
        int pageSize, 
        string sortBy = "Id", 
        bool sortDescending = false,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsQueryable();
        query = sortBy.ToUpperInvariant() switch
        {
            "NAME" => sortDescending 
                ? query.OrderByDescending(u => u.Name) 
                : query.OrderBy(u => u.Name),
            "EMAIL" => sortDescending 
                ? query.OrderByDescending(u => u.Email) 
                : query.OrderBy(u => u.Email),
            "CREATEDAT" => sortDescending 
                ? query.OrderByDescending(u => u.CreatedAt) 
                : query.OrderBy(u => u.CreatedAt),
            _ => sortDescending 
                ? query.OrderByDescending(u => u.Id) 
                : query.OrderBy(u => u.Id)
        };
        return await query
            .Skip(pageNumber)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }
}
