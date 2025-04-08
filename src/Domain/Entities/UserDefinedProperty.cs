namespace Acme.Services.VoucherManagementService.Domain.Entities;

public class UserDefinedProperty
{
    public string Id { get; set; } = string.Empty;
    public List<string> Values { get; set; } = new();
}
