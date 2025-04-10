using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Acme.Services.VoucherManagementService.Application.Models.Common;

namespace Acme.Services.VoucherManagementService.Application.Common.PipelineBehaviours;

public class LoggingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehaviour<TRequest, TResponse>> _logger;

    public LoggingBehaviour(ILogger<LoggingBehaviour<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        
        _logger.LogInformation("Handling {RequestName}", requestName);

        string requestJson;
        if (request is IContainsSensitiveData)
        {
            requestJson = "[Contains sensitive data]";
        }
        else
        {
            try
            {
                requestJson = JsonSerializer.Serialize(request);
            }
            catch (Exception ex)
            {
                requestJson = $"[Could not serialize request: {ex.Message}]";
            }
        }

        _logger.LogDebug("Request details: {RequestJson}", requestJson);

        var response = await next();

        _logger.LogInformation("Completed {RequestName}", requestName);

        return response;
    }
}
