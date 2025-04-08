using System.Net;
using System.Text.Json;

namespace Acme.Services.VoucherManagementService.Api.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IWebHostEnvironment _env;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IWebHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception has occurred");

            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            if (_env.IsDevelopment())
            {
                var response = new
                {
                    Status = httpContext.Response.StatusCode,
                    Title = "An error occurred",
                    Detail = ex.Message,
                    StackTrace = ex.StackTrace
                };

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var json = JsonSerializer.Serialize(response, options);
                await httpContext.Response.WriteAsync(json);
            }
            else
            {
                var response = new
                {
                    Status = httpContext.Response.StatusCode,
                    Title = "An error occurred",
                    Detail = "An internal server error has occurred."
                };

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var json = JsonSerializer.Serialize(response, options);
                await httpContext.Response.WriteAsync(json);
            }
        }
    }
}
