namespace Acme.Services.VoucherManagementService.Application.Models.Common;

public class UserDefinedProperty
{
    public required string Id { get; set; }
    public required List<string> Values { get; set; }
}
