using CommonTestUtilities.Builders.Application.Users;
using Shouldly;

namespace IntegrationTests.Application.Features.Users;

public class UpdateUserCommandHandlerTests(IntegrationTestAppFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task Handle_ExistingUser_ShouldUpdateUserSuccessfully()
    {
        // Arrange
        var createCommand = new CreateUserCommandBuilder().Build();
        var createdUser = await Sender.Send(createCommand);
        
        var updateCommand = new UpdateUserCommandBuilder()
            .WithId(createdUser.Value.Id)
            .Build();

        // Act
        var result = await Sender.Send(updateCommand);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.Id.ShouldBe(updateCommand.Id);
        result.Value.Name.ShouldBe(updateCommand.Name);
        result.Value.Email.ShouldBe(updateCommand.Email);
    }

    [Fact]
    public async Task Handle_NonExistentUser_ShouldReturnNotFoundError()
    {
        // Arrange
        var command = new UpdateUserCommandBuilder()
            .WithId(Guid.NewGuid())
            .Build();

        // Act
        var result = await Sender.Send(command);

        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.Type.ShouldBe(ErrorOr.ErrorType.NotFound);
    }

    [Fact]
    public async Task Handle_DuplicateEmail_ShouldReturnConflictError()
    {
        // Arrange
        var firstUser = await Sender.Send(new CreateUserCommandBuilder().Build());
        var secondUserCreate = new CreateUserCommandBuilder().Build();
        var secondUser = await Sender.Send(secondUserCreate);

        var updateCommand = new UpdateUserCommandBuilder()
            .WithId(secondUser.Value.Id)
            .WithEmail(firstUser.Value.Email) // Using first user's email
            .Build();

        // Act
        var result = await Sender.Send(updateCommand);

        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.Type.ShouldBe(ErrorOr.ErrorType.Conflict);
    }
}
