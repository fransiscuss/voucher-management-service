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
        HttpStatusCode successCode = HttpStatusCode.OK) where TError : Enum
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
            var errorCode = error.Metadata.TryGetValue("ErrorCode", out var code) && code != null
                ? Enum.Parse<TError>(code.ToString() ?? "0")
                : default;

            var problemDetails = new ProblemDetails<TError>
            {
                Status = error.Message.Contains("not found", StringComparison.OrdinalIgnoreCase) 
                    ? (int)HttpStatusCode.NotFound 
                    : (int)HttpStatusCode.BadRequest,
                Title = "Validation Error",
                Detail = error.Message,
                ErrorCode = errorCode,
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

public class ProblemDetails<TError> where TError : Enum
{
    public int Status { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Detail { get; set; } = string.Empty;
    public TError ErrorCode { get; set; } = default!;
    public string TraceId { get; set; } = string.Empty;
}
