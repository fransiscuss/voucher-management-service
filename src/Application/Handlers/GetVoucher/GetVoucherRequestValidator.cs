using Acme.Services.VoucherManagementService.Application.Common.Extensions;
using FluentValidation;

namespace Acme.Services.VoucherManagementService.Application.Handlers.GetVoucher;

public class GetVoucherRequestValidator : AbstractValidator<GetVoucherRequest>
{
    public GetVoucherRequestValidator()
    {
        RuleFor(x => x.VoucherCode)
            .NotEmpty()
            .WithMessage("Voucher code is required")
            .WithErrorCode(GetVoucherError.InvalidInput);
    }
}
