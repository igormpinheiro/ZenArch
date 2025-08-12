using Application.Features.Users.Commands;

namespace CommonTestUtilities.Builders.Application.Users;

public class DeleteUserCommandBuilder
{
    private Guid _id = Guid.NewGuid();

    public DeleteUserCommandBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public DeleteUserCommand Build() => new(_id);
}
