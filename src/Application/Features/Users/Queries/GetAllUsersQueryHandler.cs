using Application.Abstractions.Messaging;
using Application.Mappings;
using Application.Models;
using Domain.Interfaces.Repositories;
using ErrorOr;

namespace Application.Features.Users.Queries;

public sealed record GetAllUsersQuery : IQuery<IEnumerable<UserViewModel>>;

internal sealed class GetAllUsersQueryHandler(IUserRepository _userRepository)
    : IQueryHandler<GetAllUsersQuery, IEnumerable<UserViewModel>>
{
    public async Task<ErrorOr<IEnumerable<UserViewModel>>> Handle(GetAllUsersQuery request,
        CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetAllAsync(cancellationToken);
        return users.ToViewModel().ToList();
    }
}
