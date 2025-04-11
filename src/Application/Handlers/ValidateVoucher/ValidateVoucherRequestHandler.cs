using Acme.Services.VoucherManagementService.Application.Interfaces.Infrastructure;
using Acme.Services.VoucherManagementService.Application.Models.Common;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Acme.Services.VoucherManagementService.Application.Handlers.ValidateVoucher;

public class ValidateVoucherRequestHandler : IRequestHandler<ValidateVoucherRequest, Result<ValidateVoucherResponse>>
{
    private readonly IVoucherRepository _voucherRepository;
    private readonly ILogger<ValidateVoucherRequestHandler> _logger;

    public ValidateVoucherRequestHandler(
        IVoucherRepository voucherRepository,
        ILogger<ValidateVoucherRequestHandler> logger)
    {
        _voucherRepository = voucherRepository;
        _logger = logger;
    }

    public async Task<Result<ValidateVoucherResponse>> Handle(ValidateVoucherRequest request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Validating voucher with code: {VoucherCode}, issuing party: {IssuingParty}",
                request.VoucherCode, request.IssuingParty);

            var voucher = await _voucherRepository.Get(request.VoucherCode, request.IssuingParty);

            if (voucher is null)
            {
                _logger.LogWarning("Voucher not found with code: {VoucherCode}, issuing party: {IssuingParty}",
                    request.VoucherCode, request.IssuingParty);
                return new ResourceNotFoundError("Voucher not found");
            }

            var validationResult = voucher.Validate();
            bool isValid = validationResult.IsSuccess;

            if (!isValid)
            {
                string errorMessage = validationResult.Errors[0].Message;
                _logger.LogInformation("Voucher {VoucherCode} validation failed: {ErrorMessage}", 
                    request.VoucherCode, errorMessage);
                
                // Map domain error to application error
                ValidateVoucherError errorCode = errorMessage switch
                {
                    "Voucher is expired" => ValidateVoucherError.VoucherExpired,
                    "Voucher is already redeemed" => ValidateVoucherError.VoucherAlreadyRedeemed,
                    _ => ValidateVoucherError.VoucherNotActive
                };
                
                // Return response with validation details but also include error code and message
                return Result.Ok(VoucherMapper.ToResponse(voucher, false))
                    .WithError(new ValidationError(errorMessage, errorCode));
            }

            _logger.LogInformation("Voucher {VoucherCode} is valid", request.VoucherCode);
            return Result.Ok(VoucherMapper.ToResponse(voucher, true));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating voucher with code {VoucherCode}", request.VoucherCode);
            return new ValidationError("An unexpected error occurred while validating voucher", ValidateVoucherError.UnknownError);
        }
    }
}
