using Application.Abstractions.Messaging;
using Domain.Events;
using Microsoft.Extensions.Logging;

namespace Application.Features.Users.Events;

public class UserCreatedEventHandler(ILogger<UserCreatedEventHandler> log) : IDomainEventHandler<UserCreatedEvent>
{
    public Task Handle(UserCreatedEvent notification, CancellationToken cancellationToken)
    {
        log.LogInformation("User created event: {Notification}", notification);
        return Task.CompletedTask;
    }
}
