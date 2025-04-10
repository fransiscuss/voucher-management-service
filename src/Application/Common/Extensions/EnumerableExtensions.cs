using Acme.Services.VoucherManagementService.Application.Models.Common;
using Acme.Services.VoucherManagementService.Domain.Entities;

namespace Acme.Services.VoucherManagementService.Application.Common.Extensions;

public static class EnumerableExtensions
{
    public static List<UserDefinedProperty>? ToUserDefinedProperties(
        this IEnumerable<Models.Common.UserDefinedProperty>? properties)
    {
        if (properties == null || !properties.Any())
        {
            return null;
        }

        return properties.Select(p => new Domain.Entities.UserDefinedProperty
        {
            Id = p.Id,
            Values = p.Values
        }).ToList();
    }

    public static List<Models.Common.UserDefinedProperty>? ToUserDefinedPropertyModels(
        this IEnumerable<Domain.Entities.UserDefinedProperty>? properties)
    {
        if (properties == null || !properties.Any())
        {
            return null;
        }

        return properties.Select(p => new Models.Common.UserDefinedProperty
        {
            Id = p.Id,
            Values = p.Values
        }).ToList();
    }
}
