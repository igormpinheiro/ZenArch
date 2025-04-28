using Application.Abstractions.Messaging;
using Application.Mappings;
using Application.Models;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using ErrorOr;

namespace Application.Features.Users.Commands;

internal sealed class CreateUserCommandHandler(IUserRepository _userRepository)
    : ICommandHandler<CreateUserCommand, UserViewModel>
{
    public async Task<ErrorOr<UserViewModel>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = new User(request.Email, request.Name);

        await _userRepository.AddAsync(user, cancellationToken);

        return user.ToViewModel();
    }
}
