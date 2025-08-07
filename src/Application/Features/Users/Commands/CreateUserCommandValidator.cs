using FluentValidation;
using SharedKernel.Extensions;
using SharedKernel.Resources;

namespace Application.Features.Users.Commands;

internal sealed class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(user => user.Name)
            .NotEmpty().WithMessage(ResourceMessages.NAME_EMPTY)
            .MaximumLength(100).WithMessage(ResourceMessages.FIELD_TOO_LONG);
        RuleFor(user => user.Email).NotEmpty().WithMessage(ResourceMessages.EMAIL_EMPTY);
        When(user => user.Email.NotEmpty(),
            () => RuleFor(user => user.Email).EmailAddress().WithMessage(ResourceMessages.EMAIL_INVALID));
    }
}
