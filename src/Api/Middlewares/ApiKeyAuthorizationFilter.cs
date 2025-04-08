using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace Acme.Services.VoucherManagementService.Api.Middlewares;

public class ApiKeyAuthorizationFilter : IAuthorizationFilter
{
    private const string ApiKeyHeaderName = "X-API-Key";
    private readonly IConfiguration _configuration;

    public ApiKeyAuthorizationFilter(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey))
        {
            context.Result = new UnauthorizedObjectResult("API Key is missing");
            return;
        }

        var apiKey = _configuration["Authentication:ApiKey"];
        
        if (!apiKey.Equals(extractedApiKey))
        {
            context.Result = new UnauthorizedObjectResult("Invalid API Key");
            return;
        }
    }
}
