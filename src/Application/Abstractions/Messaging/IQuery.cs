using ErrorOr;
using MediatR;


namespace Application.Abstractions.Messaging;

/// <summary>
/// Interface para queries que retornam dados tipados.
/// Utiliza ErrorOr para indicar sucesso ou falha.
/// </summary>
/// <typeparam name="TResponse">Tipo do dado retornado em caso de sucesso.</typeparam>
public interface IQuery<TResponse> : IRequest<ErrorOr<TResponse>>
    where TResponse : notnull
{
}

/// <summary>
/// Interface base para handlers de queries.
/// </summary>
/// <typeparam name="TQuery">Tipo da query a ser manipulada.</typeparam>
/// <typeparam name="TResponse">Tipo do dado retornado em caso de sucesso.</typeparam>
public interface IQueryHandler<TQuery, TResponse> :
    IRequestHandler<TQuery, ErrorOr<TResponse>>
    where TQuery : IQuery<TResponse>
    where TResponse : notnull
{
}
