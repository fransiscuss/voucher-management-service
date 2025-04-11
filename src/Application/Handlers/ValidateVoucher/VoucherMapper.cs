using Acme.Services.VoucherManagementService.Application.Common.Extensions;
using Acme.Services.VoucherManagementService.Domain.Entities;

namespace Acme.Services.VoucherManagementService.Application.Handlers.ValidateVoucher;

public static class VoucherMapper
{
    public static ValidateVoucherResponse ToResponse(Voucher voucher, bool isValid)
    {
        return new ValidateVoucherResponse
        {
            Id = voucher.Id,
            Code = voucher.Code,
            Status = voucher.Status.ToString(),
            IssuingParty = voucher.IssuingParty,
            Category = voucher.Category,
            ExpiryDateUtc = voucher.ExpiryDateUtc,
            IsValid = isValid,
            UserDefinedProperties = voucher.UserDefinedProperties.ToUserDefinedPropertyModels()
        };
    }
}
