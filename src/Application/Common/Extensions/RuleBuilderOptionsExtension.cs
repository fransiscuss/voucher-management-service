using FluentValidation;

namespace Acme.Services.VoucherManagementService.Application.Common.Extensions;

public static class RuleBuilderOptionsExtension
{
    public static IRuleBuilderOptions<T, TProperty> WithErrorCode<T, TProperty>(
        this IRuleBuilderOptions<T, TProperty> rule, Enum errorCode)
    {
        return rule.WithErrorCode(errorCode.ToString());
    }
}
