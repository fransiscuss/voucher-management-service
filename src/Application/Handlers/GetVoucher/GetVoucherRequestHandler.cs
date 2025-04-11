using Acme.Services.VoucherManagementService.Application.Interfaces.Infrastructure;
using Acme.Services.VoucherManagementService.Application.Models.Common;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Acme.Services.VoucherManagementService.Application.Handlers.GetVoucher;

public class GetVoucherRequestHandler : IRequestHandler<GetVoucherRequest, Result<GetVoucherResponse>>
{
    private readonly IVoucherRepository _voucherRepository;
    private readonly ILogger<GetVoucherRequestHandler> _logger;

    public GetVoucherRequestHandler(
        IVoucherRepository voucherRepository, 
        ILogger<GetVoucherRequestHandler> logger)
    {
        _voucherRepository = voucherRepository;
        _logger = logger;
    }

    public async Task<Result<GetVoucherResponse>> Handle(GetVoucherRequest request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Getting voucher with code: {VoucherCode}", request.VoucherCode);
            
            var voucher = request.IssuingParty != null 
                ? await _voucherRepository.Get(request.VoucherCode, request.IssuingParty)
                : await _voucherRepository.GetByVoucherCode(request.VoucherCode);

            if (voucher == null)
            {
                _logger.LogWarning("Voucher with code {VoucherCode} not found", request.VoucherCode);
                return new ResourceNotFoundError("Voucher not found");
            }

            _logger.LogInformation("Voucher found: {VoucherId}", voucher.Id);
            
            var response = VoucherMapper.ToResponse(voucher);
            return Result.Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting voucher with code {VoucherCode}", request.VoucherCode);
            return new ValidationError("An unexpected error occurred", GetVoucherError.UnknownError);
        }
    }
}
