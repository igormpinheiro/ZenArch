using SharedKernel.Abstractions.Domain;

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
        EventId = Guid.NewGuid();
        OccurredOn = DateTimeOffset.UtcNow;      
    }

    public Guid EventId { get; }
    public DateTimeOffset OccurredOn { get; }
}

public sealed record AdminUserCreatedEvent : IDomainEvent
{
    public Guid UserId { get; }

    public AdminUserCreatedEvent(Guid userId)
    {
        UserId = userId;
        EventId = Guid.NewGuid();
        OccurredOn = DateTimeOffset.UtcNow;      
    }

    public Guid EventId { get; }
    public DateTimeOffset OccurredOn { get; }
}
