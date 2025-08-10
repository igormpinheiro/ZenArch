using Application.Abstractions.Messaging;
using Application.Mappings;
using Application.Models;
using Domain.Entities;
using Domain.Errors;
using Domain.Interfaces.Repositories;
using ErrorOr;

namespace Application.Features.Users.Commands;

internal sealed class CreateUserCommandHandler(IUserRepository _userRepository)
    : ICommandHandler<CreateUserCommand, UserViewModel>
{
    public async Task<ErrorOr<UserViewModel>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var emailAlreadyExists = await _userRepository.ExistsAsync(p => p.Email == request.Email, cancellationToken);
        if (emailAlreadyExists)
        {
            return UserErrors.EmailAlreadyExists;
        }

        var newUser = User.Factory.Create(request.Email, request.Name);
        await _userRepository.AddAsync(newUser, cancellationToken);
        return newUser.ToViewModel();
    }
}
