using FluentResults;
using MediatR;

namespace Acme.Services.VoucherManagementService.Application.Handlers.RedeemVoucher;

public class RedeemVoucherRequest : IRequest<Result<RedeemVoucherResponse>>
{
    public required string VoucherCode { get; set; }
    public required string IssuingParty { get; set; }
    public required string RedemptionId { get; set; }
    public required string RedemptionType { get; set; }
}
