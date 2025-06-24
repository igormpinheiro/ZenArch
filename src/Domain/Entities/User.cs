using Domain.Events;
using SharedKernel.Abstractions;

namespace Domain.Entities;

public class User : BaseEntity<Guid>
{
    public string Name { get; private set; }
    public string Email { get; private set; }

    private User()
    {
    }

    private User(Guid id, string email, string name) : base(id)
    {
        Email = email;
        Name = name;
    }

    public void UpdateName(string name)
    {
        Name = name;
    }

    public void UpdateEmail(string email)
    {
        Email = email;
    }

    public static class Factory
    {
        public static User Create(string email, string name)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = email,
                Name = name,
            };
            
            user.Raise(new UserCreatedEvent(user.Id, email, name));

            return user;
        }

        public static User Create(Guid id, string email, string name)
        {
            var user = new User(id, email, name);
            return user;
        }

        public static User CreateAdminUser(string email, string name)
        {
            var user = Create(email, name);
            user.Raise(new AdminUserCreatedEvent(user.Id));
            return user;
        }
    }
}
