using FluentValidation;
using Domain.Errors;

namespace Application.Features.Users.Commands;

internal sealed class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserCommandValidator()
    {
        RuleFor(command => command.Id)
            .NotEmpty().WithMessage(UserErrors.IdEmpty.Description);
    }
}