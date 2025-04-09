namespace Acme.Services.VoucherManagementService.Application.Models.Configuration;

public class ServiceBusSettings
{
    public string VoucherRedeemConnectionString { get; set; } = string.Empty;
    public string VoucherRedeemTopicName { get; set; } = string.Empty;
    public bool IsAlternateRegionEnabled { get; set; }
    public string? VoucherRedeemConnectionStringAlternate { get; set; }
}
