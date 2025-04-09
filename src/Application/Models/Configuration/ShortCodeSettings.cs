namespace Acme.Services.VoucherManagementService.Application.Models.Configuration;

public class ShortCodeSettings
{
    public bool EnableShortCodeGeneration { get; set; } = true;
    public int Length { get; set; } = 8;
    public string CharacterSet { get; set; } = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
}
