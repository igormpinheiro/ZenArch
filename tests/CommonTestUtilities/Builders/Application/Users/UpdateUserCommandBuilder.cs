using Application.Features.Users.Commands;
using Bogus;

namespace CommonTestUtilities.Builders.Application.Users;

public class UpdateUserCommandBuilder
{
    private Guid _id = Guid.NewGuid();
    private string _email = new Faker().Internet.Email();
    private string _name = new Faker().Name.FullName();

    public UpdateUserCommandBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public UpdateUserCommandBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public UpdateUserCommandBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public UpdateUserCommand Build() => new(_id, _email, _name);
}
