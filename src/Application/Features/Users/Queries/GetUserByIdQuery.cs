using Application.Abstractions.Messaging;
using Application.Mappings;
using Application.Models;
using Domain.Interfaces.Repositories;
using ErrorOr;

namespace Application.Features.Users.Queries;

public sealed record GetUserByIdQuery(Guid UserId) : IQuery<UserViewModel>;

internal sealed class GetUserByIdQueryHandler(IUserRepository _userRepository) : IQueryHandler<GetUserByIdQuery, UserViewModel>
{
    public async Task<ErrorOr<UserViewModel>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user == null)
        {
            return Error.NotFound();
        }

        return user.ToViewModel();
    }
}
