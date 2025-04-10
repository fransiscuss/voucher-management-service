using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Acme.Services.VoucherManagementService.Application.Common.PipelineBehaviours;
using Acme.Services.VoucherManagementService.Application.Models.Configuration;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Acme.Services.VoucherManagementService.Application;

[ExcludeFromCodeCoverage]
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // behaviours need to be registered in the order they are executed
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        
        services.Configure<GenericSetting>(configuration.GetSection("Generic"));
        services.Configure<AuthenticationSetting>(configuration.GetSection("Authentication"));
        services.Configure<ShortCodeSettings>(configuration.GetSection("ShortCode"));
        services.Configure<ServiceBusSettings>(configuration.GetSection("ServiceBus"));
        
        return services;
    }
}
