using SharedKernel.Abstractions;

namespace Domain.Events;

public sealed record UserCreatedEvent : IDomainEvent
{
    public Guid UserId { get; }
    public string Email { get; }
    public string Name { get; }

    public UserCreatedEvent(Guid userId, string email, string name)
    {
        UserId = userId;
        Email = email;
        Name = name;
    }
}

public sealed record AdminUserCreatedEvent : IDomainEvent
{
    public Guid UserId { get; }

    public AdminUserCreatedEvent(Guid userId)
    {
        UserId = userId;
    }
}
