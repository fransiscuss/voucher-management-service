using Acme.Services.VoucherManagementService.Application.Common.Extensions;
using FluentValidation;

namespace Acme.Services.VoucherManagementService.Application.Handlers.ValidateVoucher;

public class ValidateVoucherRequestValidator : AbstractValidator<ValidateVoucherRequest>
{
    public ValidateVoucherRequestValidator()
    {
        RuleFor(x => x.VoucherCode)
            .NotEmpty()
            .WithMessage("Voucher code is required")
            .WithErrorCode(ValidateVoucherError.InvalidInput);

        RuleFor(x => x.IssuingParty)
            .NotEmpty()
            .WithMessage("Issuing party is required")
            .WithErrorCode(ValidateVoucherError.InvalidInput);
    }
}
