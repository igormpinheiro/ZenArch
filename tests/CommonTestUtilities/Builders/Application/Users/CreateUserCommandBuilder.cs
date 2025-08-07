using Application.Features.Users.Commands;
using Bogus;

namespace CommonTestUtilities.Builders.Application.Users;

public class CreateUserCommandBuilder
{
    private string? _name;
    private string? _email;

    public CreateUserCommandBuilder WithName(string? name)
    {
        _name = name;
        return this;
    }

    public CreateUserCommandBuilder WithEmail(string? email)
    {
        _email = email;
        return this;
    }
    
    public CreateUserCommand Build()
    {
        return new Faker<CreateUserCommand>()
            .CustomInstantiator(f => new CreateUserCommand(
                Email: _email ?? f.Internet.Email(),
                Name: _name ?? f.Person.FullName[..Math.Min(f.Person.FullName.Length, 100)]))
            .Generate();
    }
}
