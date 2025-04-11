using Acme.Services.VoucherManagementService.Application.Common.Extensions;
using Acme.Services.VoucherManagementService.Domain.Entities;

namespace Acme.Services.VoucherManagementService.Application.Handlers.GetVoucher;

public static class VoucherMapper
{
    public static GetVoucherResponse ToResponse(Voucher voucher)
    {
        return new GetVoucherResponse
        {
            Id = voucher.Id,
            Code = voucher.Code,
            Status = voucher.Status.ToString(),
            IssuingParty = voucher.IssuingParty,
            Category = voucher.Category,
            BatchNumber = voucher.BatchNumber,
            ExpiryDateUtc = voucher.ExpiryDateUtc,
            CreatedDateUtc = voucher.CreatedDateUtc,
            RedemptionId = voucher.RedemptionId,
            RedemptionType = voucher.RedemptionType,
            RedeemedDateUtc = voucher.RedeemedDateUtc,
            UserDefinedProperties = voucher.UserDefinedProperties.ToUserDefinedPropertyModels()
        };
    }
}
