using Application.Features.Users.Commands;
using CommonTestUtilities.Builders.Application.Users;
using CommonTestUtilities.Builders.Persistence.Repositories;
using Shouldly;

namespace UnitTests.Application.Features.Users;

public class CreateUserCommandHandlerTests
{
    [Fact]
    public async Task Success()
    {
        var request = new CreateUserCommandBuilder().Build();

        var handler = CreateHandler();

        var result = await handler.Handle(request, CancellationToken.None);

        result.IsError.ShouldBeFalse();
        result.Value.Name.ShouldBe(request.Name);
        result.Value.Email.ShouldBe(request.Email);
    }
    
    private static CreateUserCommandHandler CreateHandler()
    {
        var userRepository = UserRepositoryBuilder.Build();
        return new CreateUserCommandHandler(userRepository);
    }
}
