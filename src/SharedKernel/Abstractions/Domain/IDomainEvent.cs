using MediatR;

namespace SharedKernel.Abstractions.Domain;

public interface IDomainEvent : INotification
{
    Guid Id { get; }
    DateTime OccurredOn { get; }
}