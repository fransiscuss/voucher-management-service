using FluentResults;
using MediatR;

namespace Acme.Services.VoucherManagementService.Application.Handlers.ValidateVoucher;

public class ValidateVoucherRequest : IRequest<Result<ValidateVoucherResponse>>
{
    public required string VoucherCode { get; set; }
    public required string IssuingParty { get; set; }
}
