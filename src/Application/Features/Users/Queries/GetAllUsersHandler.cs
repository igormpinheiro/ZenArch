using Application.Abstractions.Messaging;
using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Application.Features.Users.Queries;

public sealed record GetAllUsersQuery : IQuery<IEnumerable<UserResponse>>;

internal sealed class GetAllUsersHandler(ApplicationDbContext _context) : IQueryHandler<GetAllUsersQuery, IEnumerable<UserResponse>>
{
    public async Task<ErrorOr<IEnumerable<UserResponse>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _context.Users.ToListAsync(cancellationToken);
        
        var response = new List<UserResponse>();
        
        if (users is null || users.Count == 0)
        {
            return response;
        }
        
        return users.Select(user => new UserResponse(user.Id, user.Name, user.Email)).ToList();
    }
}
