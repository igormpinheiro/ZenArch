using MediatR;

namespace SharedKernel.Abstractions.Domain;

public interface IDomainEvent : INotification
{
    Guid EventId { get; }
    DateTimeOffset OccurredOn { get; }
}