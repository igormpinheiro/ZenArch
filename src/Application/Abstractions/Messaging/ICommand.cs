using ErrorOr;
using MediatR;

namespace Application.Abstractions.Messaging;

/// <summary>
/// Interface para commands que não retornam dados específicos.
/// Utiliza ErrorOr{Success} para indicar sucesso ou falha da operação.
/// </summary>
public interface ICommand : IRequest<ErrorOr<Success>>
{ }

/// <summary>
/// Interface para commands que retornam dados tipados.
/// </summary>
/// <typeparam name="TResponse">Tipo do dado retornado em caso de sucesso.</typeparam>
public interface ICommand<TResponse> : IRequest<ErrorOr<TResponse>>
    where TResponse : notnull
{ }

/// <summary>
/// Interface base para handlers de commands que não retornam dados específicos.
/// </summary>
/// <typeparam name="TCommand">Tipo do command a ser manipulado.</typeparam>
public interface ICommandHandler<TCommand> : IRequestHandler<TCommand, ErrorOr<Success>>
    where TCommand : ICommand
{ }

/// <summary>
/// Interface base para handlers de commands que retornam dados tipados.
/// </summary>
/// <typeparam name="TCommand">Tipo do command a ser manipulado.</typeparam>
/// <typeparam name="TResponse">Tipo do dado retornado em caso de sucesso.</typeparam>
public interface ICommandHandler<TCommand, TResponse> :
    IRequestHandler<TCommand, ErrorOr<TResponse>>
    where TCommand : ICommand<TResponse>
    where TResponse : notnull
{ }
