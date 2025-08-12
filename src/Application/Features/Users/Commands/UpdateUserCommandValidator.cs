using FluentValidation;
using SharedKernel.Extensions;
using Domain.Errors;

namespace Application.Features.Users.Commands;

internal sealed class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(command => command.Id)
            .NotEmpty().WithMessage(UserErrors.IdEmpty.Description);

        RuleFor(user => user.Name)
            .NotEmpty().WithMessage(UserErrors.NameEmpty.Description)
            .MaximumLength(100).WithMessage(UserErrors.NameTooLong.Description);
        
        RuleFor(user => user.Email)
            .NotEmpty().WithMessage(UserErrors.EmailEmpty.Description);
        
        When(user => user.Email.NotEmpty(),
            () => RuleFor(user => user.Email)
                .EmailAddress().WithMessage(UserErrors.EmailInvalid.Description));
    }
}