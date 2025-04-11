using Acme.Services.VoucherManagementService.Application.Models.Common;

namespace Acme.Services.VoucherManagementService.Application.Handlers.ValidateVoucher;

public class ValidateVoucherResponse
{
    public string Id { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string IssuingParty { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public DateTime ExpiryDateUtc { get; set; }
    public bool IsValid { get; set; }
    public List<UserDefinedProperty>? UserDefinedProperties { get; set; }
}
