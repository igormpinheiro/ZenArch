using FluentValidation;

namespace Application.Features.Users.Commands;

internal sealed class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty().WithErrorCode("NameIsRequired").WithMessage("Name is required!")
            .MaximumLength(100).WithErrorCode("NameLengthExceeded").WithMessage("Name is too long!");

        RuleFor(c => c.Email)
            .NotEmpty().WithErrorCode("EmailIsRequired").WithMessage("Email is required!")
            .EmailAddress().WithErrorCode("EmailInvalid").WithMessage("Email is invalid!");
    }
}
