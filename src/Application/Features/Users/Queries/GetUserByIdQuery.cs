using Application.Abstractions.Messaging;
using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Application.Features.Users.Queries;

public sealed record GetUserByIdQuery(Guid UserId) : IQuery<UserResponse>;

internal sealed class GetUserByIdQueryHandler(ApplicationDbContext _context) : IQueryHandler<GetUserByIdQuery, UserResponse>
{
    public async Task<ErrorOr<UserResponse>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.SingleOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);

        if (user == null)
        {
            return Error.NotFound();
        }
        
        return new UserResponse(user.Id, user.Name, user.Email);
    }
} 
