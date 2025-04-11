namespace Acme.Services.VoucherManagementService.Infrastructure.Settings;

public class CosmosDbSetting
{
    public string DocumentEndpoint { get; set; } = string.Empty;
    public string AuthKey { get; set; } = string.Empty;
    public string DatabaseId { get; set; } = string.Empty;
    public string VoucherContainerId { get; set; } = string.Empty;
    public int BulkVoucherInsertRetryAttempts { get; set; } = 3;
}
