using Application.Features.Users.Commands;
using CommonTestUtilities.Builders.Application.Users;
using SharedKernel.Resources;
using Shouldly;

namespace UnitTests.Application.Features.Users;

public class CreateUserCommandValidatorTests
{
    private readonly CreateUserCommandValidator _validator = new();

    [Fact]
    public void Sucess()
    {
        var request = new CreateUserCommandBuilder().Build();

        var result = _validator.Validate(request);

        result.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Failure_EmptyName_ShouldBeInvalid(string? name)
    {
        var request = new CreateUserCommandBuilder()
            .WithName(name)
            .Build();

        var result = _validator.Validate(request);
        
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldHaveSingleItem();
        result.Errors.ShouldContain(e => e.ErrorMessage == ResourceMessages.NAME_EMPTY);
    }

    [Fact]
    public void Failure_LongName_ShouldBeInvalid()
    {
        var request = new CreateUserCommandBuilder()
            .WithName(new string('A', 101))
            .Build();

        var result = _validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldHaveSingleItem();
        result.Errors.ShouldContain(e => e.ErrorMessage == ResourceMessages.FIELD_TOO_LONG);
    }

    [Fact]
    public void Failure_InvalidEmail_ShouldBeInvalid()
    {
        var request = new CreateUserCommandBuilder()
            .WithEmail("invalid-email")
            .Build();

        var result = _validator.Validate(request);
        
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldHaveSingleItem();
        result.Errors.ShouldContain(e => e.ErrorMessage == ResourceMessages.EMAIL_INVALID);
    }
    
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Failure_EmptyEmail_ShouldBeInvalid(string? email)
    {
        var request = new CreateUserCommandBuilder()
            .WithEmail(email)
            .Build();

        // var request = new CreateUserCommand(email, "as");
        var result = _validator.Validate(request);
        
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldHaveSingleItem();
        result.Errors.ShouldContain(e => e.ErrorMessage == ResourceMessages.EMAIL_EMPTY);
    }
}
