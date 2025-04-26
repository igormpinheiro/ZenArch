using MediatR;

namespace Application.Abstractions.Messaging;

/// <summary>
/// Interface para eventos.
/// </summary>
public interface IEvent : INotification
{ }

/// <summary>
/// Interface base para handlers de eventos.
/// </summary>
/// <typeparam name="TEvent">Tipo do evento a ser manipulado.</typeparam>
public interface IEventHandler<in TEvent> : INotificationHandler<TEvent>
    where TEvent : IEvent
{ }
