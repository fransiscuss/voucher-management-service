namespace Acme.Services.VoucherManagementService.Application.Interfaces;

public interface ICodeGenerationService
{
    Task<List<string>> Generate(int count);
}
