using CommonTestUtilities.Builders.Application.Users;
using Domain.Errors;
using Shouldly;

namespace IntegrationTests.Application.Features.Users;

public class CreateUserCommandHandlerTests(IntegrationTestAppFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task Handle_ValidCommand_ShouldCreateUserSuccessfully()
    {
        // Arrange
        var command = new CreateUserCommandBuilder().Build();

        // Act
        var result = await Sender.Send(command);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.Name.ShouldBe(command.Name);
        result.Value.Email.ShouldBe(command.Email);
    }

    [Fact]
    public async Task Handle_DuplicateEmail_ShouldReturnConflictError()
    {
        // Arrange
        var command = new CreateUserCommandBuilder().Build();
        await Sender.Send(command); // Create first user

        var duplicateCommand = new CreateUserCommandBuilder()
            .WithEmail(command.Email)
            .Build();

        // Act
        var result = await Sender.Send(duplicateCommand);

        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.Type.ShouldBe(ErrorOr.ErrorType.Conflict);
        result.FirstError.Description.ShouldBe(UserErrors.EmailAlreadyExists.Description);
    }
}