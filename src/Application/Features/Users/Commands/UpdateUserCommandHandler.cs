using Application.Abstractions.Messaging;
using Application.Mappings;
using Application.Models;
using Domain.Interfaces.Repositories;
using ErrorOr;
using SharedKernel.Abstractions;

namespace Application.Features.Users.Commands;

public sealed record UpdateUserCommand(Guid Id, string Email, string Name) : ICommand<UserViewModel>;

internal sealed class UpdateUserCommandHandler(IUserRepository repository, IUnitOfWork unitOfWork)
    : ICommandHandler<UpdateUserCommand, UserViewModel>
{
    public async Task<ErrorOr<UserViewModel>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await repository.GetByIdAsync(request.Id, cancellationToken);

        if (user is null)
        {
            return Error.Conflict("User.NotFound", "User not found");
        }

        var emailAlreadyExists = await repository.ExistsAsync(p => p.Email == request.Email, cancellationToken);

        if (user.Email != request.Email && emailAlreadyExists)
        {
            return Error.Conflict("User.EmailAlreadyExists", "Email already exists");
        }

        user.UpdateName(request.Name);
        user.UpdateEmail(request.Email);

        await repository.UpdateAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return user.ToViewModel();
    }
}
