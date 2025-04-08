using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Acme.Services.VoucherManagementService.Domain.Entities;

[JsonConverter(typeof(StringEnumConverter))]
public enum VoucherStatus
{
    Active,
    Redeemed,
    Expired
}
