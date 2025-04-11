using Acme.Services.VoucherManagementService.Application.Interfaces;
using Acme.Services.VoucherManagementService.Application.Interfaces.Infrastructure;

namespace Acme.Services.VoucherManagementService.Application.Handlers.CreateVouchersBatch.CodeGeneration;

public class UniqueCodeGenerationService : ICodeGenerationService
{
    private readonly IVoucherRepository _voucherRepository;
    
    public UniqueCodeGenerationService(IVoucherRepository voucherRepository)
    {
        _voucherRepository = voucherRepository;
    }
    
    public async Task<List<string>> Generate(int count)
    {
        var generatedCodes = new HashSet<string>();
        
        while (generatedCodes.Count < count)
        {
            var code = GenerateUniqueCode();
            
            // Check if code already exists in the repository
            var existingVoucher = await _voucherRepository.GetByVoucherCode(code);
            
            if (existingVoucher == null && !generatedCodes.Contains(code))
            {
                generatedCodes.Add(code);
            }
        }
        
        return generatedCodes.ToList();
    }
    
    private string GenerateUniqueCode()
    {
        // Generate a GUID and format it to a readable string
        return Guid.NewGuid().ToString("N").ToUpper().Substring(0, 16);
    }
}
