using Acme.Services.VoucherManagementService.Application.Interfaces;
using Acme.Services.VoucherManagementService.Application.Interfaces.Infrastructure;
using Acme.Services.VoucherManagementService.Application.Models.Configuration;
using Microsoft.Extensions.Options;

namespace Acme.Services.VoucherManagementService.Application.Handlers.CreateVouchersBatch.CodeGeneration;

public class UniqueShortCodeGenerationService : ICodeGenerationService
{
    private readonly IVoucherRepository _voucherRepository;
    private readonly ShortCodeSettings _settings;
    private readonly Random _random = new();
    
    public UniqueShortCodeGenerationService(
        IVoucherRepository voucherRepository,
        IOptions<ShortCodeSettings> settings)
    {
        _voucherRepository = voucherRepository;
        _settings = settings.Value;
    }
    
    public async Task<List<string>> Generate(int count)
    {
        if (!_settings.EnableShortCodeGeneration)
        {
            throw new InvalidOperationException("Short code generation is disabled");
        }
        
        var generatedCodes = new HashSet<string>();
        
        // Maximum attempts to prevent infinite loops
        int maxAttempts = count * 10;
        int attempts = 0;
        
        while (generatedCodes.Count < count && attempts < maxAttempts)
        {
            attempts++;
            var code = GenerateShortCode();
            
            // Check if code already exists in the repository
            var existingVoucher = await _voucherRepository.GetByVoucherCode(code);
            
            if (existingVoucher == null && !generatedCodes.Contains(code))
            {
                generatedCodes.Add(code);
            }
        }
        
        if (generatedCodes.Count < count)
        {
            throw new InvalidOperationException($"Could only generate {generatedCodes.Count} unique codes after {maxAttempts} attempts");
        }
        
        return generatedCodes.ToList();
    }
    
    private string GenerateShortCode()
    {
        var chars = _settings.CharacterSet.ToCharArray();
        var result = new char[_settings.Length];
        
        for (int i = 0; i < _settings.Length; i++)
        {
            result[i] = chars[_random.Next(chars.Length)];
        }
        
        return new string(result);
    }
}
