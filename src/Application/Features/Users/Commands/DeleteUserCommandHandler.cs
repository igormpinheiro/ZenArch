using Application.Abstractions.Messaging;
using Domain.Errors;
using Domain.Interfaces.Repositories;
using ErrorOr;
using SharedKernel.Abstractions;

namespace Application.Features.Users.Commands;

internal sealed class DeleteUserCommandHandler(IUserRepository repository, IUnitOfWork unitOfWork)
    : ICommandHandler<DeleteUserCommand>
{
    public async Task<ErrorOr<Success>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (user is null)
        {
            return UserErrors.NotFound;
        }
        await repository.DeleteAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success;
    }
}