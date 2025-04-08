using Microsoft.OpenApi.Models;

namespace Acme.Services.VoucherManagementService.Api.Extensions;

public static class WebApplicationBuilderExtension
{
    public static WebApplicationBuilder ConfigureSwagger(this WebApplicationBuilder builder)
    {
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Voucher Management Service API",
                Version = "v1",
                Description = "Service for managing the lifecycle of vouchers",
                Contact = new OpenApiContact
                {
                    Name = "Acme Services",
                    Email = "support@acmeservices.example.com"
                }
            });

            c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
            {
                Description = "API Key authentication",
                Name = "X-API-Key",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "ApiKeyScheme"
            });

            var key = new OpenApiSecurityScheme()
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                },
                In = ParameterLocation.Header
            };

            var requirement = new OpenApiSecurityRequirement
            {
                { key, new List<string>() }
            };

            c.AddSecurityRequirement(requirement);
            c.EnableAnnotations();
        });

        return builder;
    }

    public static WebApplicationBuilder ConfigureConfiguration(this WebApplicationBuilder builder)
    {
        // Add App Configuration or other configuration providers here if needed
        return builder;
    }

    public static WebApplicationBuilder RegisterAuthentication(this WebApplicationBuilder builder)
    {
        // Add authentication configuration here if needed, we're using a simple API key in this example
        return builder;
    }
}
