using Application.Abstractions.Messaging;
using Domain.Interfaces.Repositories;
using ErrorOr;

namespace Application.Features.Users.Queries;

public sealed record GetUserByIdQuery(Guid UserId) : IQuery<UserResponse>;

internal sealed class GetUserByIdQueryHandler(IUserRepository _userRepository) : IQueryHandler<GetUserByIdQuery, UserResponse>
{
    public async Task<ErrorOr<UserResponse>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user == null)
        {
            return Error.NotFound();
        }
        
        return new UserResponse(user.Id, user.Name, user.Email);
    }
} 
