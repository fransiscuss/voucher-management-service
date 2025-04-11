using Acme.Services.VoucherManagementService.Application.Models.Common;

namespace Acme.Services.VoucherManagementService.Application.Handlers.GetVoucher;

public class GetVoucherResponse
{
    public string Id { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string IssuingParty { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string BatchNumber { get; set; } = string.Empty;
    public DateTime ExpiryDateUtc { get; set; }
    public DateTime CreatedDateUtc { get; set; }
    public string? RedemptionId { get; set; }
    public string? RedemptionType { get; set; }
    public DateTime? RedeemedDateUtc { get; set; }
    public List<UserDefinedProperty>? UserDefinedProperties { get; set; }
}
