using Acme.Services.VoucherManagementService.Application.Models.Configuration;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text.Json;

namespace Acme.Services.VoucherManagementService.Api.Middlewares;

public class ApiKeyAuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IOptionsMonitor<AuthenticationSetting> _settings;
    private readonly ILogger<ApiKeyAuthenticationMiddleware> _logger;
    private const string ApiKeyHeaderName = "X-API-Key";

    public ApiKeyAuthenticationMiddleware(
        RequestDelegate next,
        IOptionsMonitor<AuthenticationSetting> settings,
        ILogger<ApiKeyAuthenticationMiddleware> logger)
    {
        _next = next;
        _settings = settings;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Skip API key for health check or Swagger
        var path = context.Request.Path.ToString().ToLower();
        if (path.StartsWith("/api/health") || 
            path.StartsWith("/swagger") || 
            path.StartsWith("/health"))
        {
            await _next(context);
            return;
        }

        // Check for API key
        if (!context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey))
        {
            _logger.LogWarning("API key was not provided for request: {Path}", context.Request.Path);
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            context.Response.ContentType = "application/json";
            
            var response = new { error = "API Key is missing" };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            return;
        }

        var apiKey = _settings.CurrentValue.ApiKey;
        if (apiKey != extractedApiKey)
        {
            _logger.LogWarning("Invalid API key provided for request: {Path}", context.Request.Path);
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            context.Response.ContentType = "application/json";
            
            var response = new { error = "Invalid API Key" };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            return;
        }

        await _next(context);
    }
}

public static class ApiKeyAuthenticationMiddlewareExtensions
{
    public static IApplicationBuilder UseApiKeyAuthentication(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ApiKeyAuthenticationMiddleware>();
    }
}
