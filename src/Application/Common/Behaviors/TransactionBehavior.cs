using Application.Abstractions.Messaging;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel.Abstractions;

namespace Application.Common.Behaviors;

/// <summary>
/// Pipeline behavior do MediatR para gerenciar transações automaticamente para comandos
/// que implementam ITransactionalCommand
/// </summary>
/// <typeparam name="TRequest">Tipo da requisição</typeparam>
/// <typeparam name="TResponse">Tipo da resposta encapsulada em ErrorOr</typeparam>
public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;

    public TransactionBehavior(IUnitOfWork unitOfWork, ILogger<TransactionBehavior<TRequest, TResponse>> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // Verificar se o request é um comando transacional
        if (request is not ITransactionalCommand)
        {
            // Se não for um comando transacional, apenas passa adiante
            return await next();
        }

        var requestType = request.GetType().Name;
        _logger.LogInformation("[Transaction] Iniciando transação para {RequestType}", requestType);

        try
        {
            // Executar o próximo handler dentro de uma transação
            var response = await _unitOfWork.ExecuteTransactionAsync(async () =>
                // Chamar o próximo handler na pipeline
                await next(), cancellationToken);

            _logger.LogInformation("[Transaction] Transação concluída com sucesso para {RequestType}", requestType);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Transaction] Erro durante a transação para {RequestType}", requestType);
            throw;
        }
    }
}
