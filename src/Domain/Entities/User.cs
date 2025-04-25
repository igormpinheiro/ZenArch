using SharedKernel.Abstractions;

namespace Domain.Entities;

public class User : BaseEntity
{
    public string Name { get; private set; }
    public string Email { get; private set; }

    private User()
    {
    }

    public User(Guid id, string email, string name) : base(id)
    {
        Email = email;
        Name = name;
    }
}
