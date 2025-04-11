using Acme.Services.VoucherManagementService.Application.Common.Extensions;
using FluentValidation;

namespace Acme.Services.VoucherManagementService.Application.Handlers.RedeemVoucher;

public class RedeemVoucherRequestValidator : AbstractValidator<RedeemVoucherRequest>
{
    public RedeemVoucherRequestValidator()
    {
        RuleFor(x => x.VoucherCode)
            .NotEmpty()
            .WithMessage("Voucher code is required")
            .WithErrorCode(RedeemVoucherError.InvalidInput);

        RuleFor(x => x.IssuingParty)
            .NotEmpty()
            .WithMessage("Issuing party is required")
            .WithErrorCode(RedeemVoucherError.InvalidInput);

        RuleFor(x => x.RedemptionId)
            .NotEmpty()
            .WithMessage("Redemption ID is required")
            .WithErrorCode(RedeemVoucherError.InvalidInput);

        RuleFor(x => x.RedemptionType)
            .NotEmpty()
            .WithMessage("Redemption type is required")
            .WithErrorCode(RedeemVoucherError.InvalidInput);
    }
}
