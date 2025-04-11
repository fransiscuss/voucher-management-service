using FluentResults;
using MediatR;

namespace Acme.Services.VoucherManagementService.Application.Handlers.GetVoucher;

public class GetVoucherRequest : IRequest<Result<GetVoucherResponse>>
{
    public required string VoucherCode { get; set; }
    public string? IssuingParty { get; set; }
}
