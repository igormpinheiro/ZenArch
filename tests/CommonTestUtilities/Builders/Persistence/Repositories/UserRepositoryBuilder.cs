using Domain.Interfaces.Repositories;
using Moq;

namespace CommonTestUtilities.Builders.Persistence.Repositories;

public class UserRepositoryBuilder
{
    public static IUserRepository Build()
    {
        var mock = new Mock<IUserRepository>();

        return mock.Object;
    }
}
