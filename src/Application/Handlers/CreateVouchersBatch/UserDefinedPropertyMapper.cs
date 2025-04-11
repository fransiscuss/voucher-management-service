using Acme.Services.VoucherManagementService.Application.Common.Extensions;

namespace Acme.Services.VoucherManagementService.Application.Handlers.CreateVouchersBatch;

public static class UserDefinedPropertyMapper
{
    public static List<Domain.Entities.UserDefinedProperty>? ToUserDefinedProperties(
        this IEnumerable<Models.Common.UserDefinedProperty>? properties)
    {
        return properties.ToUserDefinedProperties();
    }
}
