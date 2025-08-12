using Application.Features.Users.Queries;
using CommonTestUtilities.Builders.Application.Users;
using Shouldly;

namespace IntegrationTests.Application.Features.Users;

public class DeleteUserCommandHandlerTests(IntegrationTestAppFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task Handle_ExistingUser_ShouldDeleteUserSuccessfully()
    {
        // Arrange
        var createCommand = new CreateUserCommandBuilder().Build();
        var createdUser = await Sender.Send(createCommand);
        
        var deleteCommand = new DeleteUserCommandBuilder()
            .WithId(createdUser.Value.Id)
            .Build();

        // Act
        var result = await Sender.Send(deleteCommand);

        // Assert
        result.IsError.ShouldBeFalse();
        
        // Verify user is deleted by trying to fetch it
        var getQuery = new GetUserByIdQuery(createdUser.Value.Id);
        var getResult = await Sender.Send(getQuery);
        getResult.IsError.ShouldBeTrue();
        getResult.FirstError.Type.ShouldBe(ErrorOr.ErrorType.NotFound);
    }

    [Fact]
    public async Task Handle_NonExistentUser_ShouldReturnNotFoundError()
    {
        // Arrange
        var command = new DeleteUserCommandBuilder()
            .WithId(Guid.NewGuid())
            .Build();

        // Act
        var result = await Sender.Send(command);

        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.Type.ShouldBe(ErrorOr.ErrorType.NotFound);
    }
}
