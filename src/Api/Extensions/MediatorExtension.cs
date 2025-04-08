using System.Net;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Acme.Services.VoucherManagementService.Api.Extensions;

public static class MediatorExtension
{
    public static async Task<IActionResult> SendWithErrorHandling<TResponse, TError>(
        this IMediator mediator,
        IRequest<Result<TResponse>> request,
        HttpStatusCode successCode = HttpStatusCode.OK) where TError : struct, Enum // Ensure TError is a non-nullable value type
    {
        var result = await mediator.Send(request);

        if (result.IsSuccess)
        {
            if (successCode == HttpStatusCode.NoContent)
            {
                return new NoContentResult();
            }

            return new ObjectResult(result.Value)
            {
                StatusCode = (int)successCode
            };
        }

        var error = result.Errors.FirstOrDefault();

        if (error is FluentResults.IReason)
        {
            var problemDetails = new ProblemDetails<TError>
            {
                Status = error.Message.Contains("not found", StringComparison.OrdinalIgnoreCase)
                    ? (int)HttpStatusCode.NotFound
                    : (int)HttpStatusCode.BadRequest,
                Title = "Validation Error",
                Detail = error.Message,
                ErrorCode = Enum.TryParse<TError>(error.Metadata.GetValueOrDefault("ErrorCode")?.ToString(), out var parsedErrorCode)
                    ? parsedErrorCode
                    : default, // Use Enum.TryParse to safely parse the value
                TraceId = Guid.NewGuid().ToString()
            };

            return new ObjectResult(problemDetails)
            {
                StatusCode = problemDetails.Status
            };
        }

        return new BadRequestObjectResult(result.Errors);
    }
}

public class ProblemDetails<TError> where TError : struct, Enum // Ensure TError is a non-nullable value type
{
    public int Status { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Detail { get; set; } = string.Empty;
    public TError ErrorCode { get; set; } = default!;
    public string TraceId { get; set; } = string.Empty;
}
