namespace Acme.Services.VoucherManagementService.Application.Interfaces;

public interface ICodeGenerationServiceFactory
{
    ICodeGenerationService GetCodeGenerationService(string codeGenerationType);
}
