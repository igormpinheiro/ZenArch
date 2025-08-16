using Domain.Errors;
using FluentValidation;
using SharedKernel.Extensions;

namespace Application.Features.Users.Commands;

internal sealed class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
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
