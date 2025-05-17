using CommonTestUtilities.Builders.Application.Users;
using Shouldly;

namespace IntegrationTests.Application.Features.Users;

public class CreateUserCommandHandlerTests(IntegrationTestAppFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task Success()
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
}
