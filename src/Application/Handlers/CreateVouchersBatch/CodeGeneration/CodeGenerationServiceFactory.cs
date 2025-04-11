using Acme.Services.VoucherManagementService.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Acme.Services.VoucherManagementService.Application.Handlers.CreateVouchersBatch.CodeGeneration;

public class CodeGenerationServiceFactory : ICodeGenerationServiceFactory
{
    private readonly IEnumerable<ICodeGenerationService> _services;
    private readonly ILogger<CodeGenerationServiceFactory> _logger;

    public CodeGenerationServiceFactory(
        IEnumerable<ICodeGenerationService> services,
        ILogger<CodeGenerationServiceFactory> logger)
    {
        _services = services;
        _logger = logger;
    }

    public ICodeGenerationService GetCodeGenerationService(string codeGenerationType)
    {
        var service = codeGenerationType switch
        {
            CodeGenerationType.ShortCode => _services.FirstOrDefault(s => s is UniqueShortCodeGenerationService),
            CodeGenerationType.UniqueCode => _services.FirstOrDefault(s => s is UniqueCodeGenerationService),
            _ => _services.FirstOrDefault(s => s is UniqueCodeGenerationService)
        };

        if (service == null)
        {
            _logger.LogWarning("Code generation service for type {CodeGenerationType} not found. Using default.", codeGenerationType);
            service = _services.FirstOrDefault(s => s is UniqueCodeGenerationService);
            
            if (service == null)
            {
                throw new InvalidOperationException("No code generation service available");
            }
        }

        return service;
    }
}
