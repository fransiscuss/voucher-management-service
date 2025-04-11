using Acme.Services.VoucherManagementService.Application.Common.Constants;
using Acme.Services.VoucherManagementService.Application.Interfaces.Infrastructure;
using Acme.Services.VoucherManagementService.Application.Models.Common;
using Acme.Services.VoucherManagementService.Application.Models.Configuration;
using Acme.Services.VoucherManagementService.Domain.Messaging.ServiceBus;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UserDefinedProperty = Acme.Services.VoucherManagementService.Domain.Messaging.ServiceBus.UserDefinedProperty;

namespace Acme.Services.VoucherManagementService.Application.Handlers.RedeemVoucher;

public class RedeemVoucherRequestHandler : IRequestHandler<RedeemVoucherRequest, Result<RedeemVoucherResponse>>
{
    private readonly IVoucherRepository _voucherRepository;
    private readonly IMessagePublishService _messagePublishService;
    private readonly ServiceBusSettings _serviceBusSettings;
    private readonly ILogger<RedeemVoucherRequestHandler> _logger;

    public RedeemVoucherRequestHandler(
        IVoucherRepository voucherRepository,
        IMessagePublishService messagePublishService,
        IOptions<ServiceBusSettings> serviceBusSettings,
        ILogger<RedeemVoucherRequestHandler> logger)
    {
        _voucherRepository = voucherRepository;
        _messagePublishService = messagePublishService;
        _serviceBusSettings = serviceBusSettings.Value;
        _logger = logger;
    }

    public async Task<Result<RedeemVoucherResponse>> Handle(RedeemVoucherRequest request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Redeeming voucher with code: {VoucherCode}, issuing party: {IssuingParty}", 
                request.VoucherCode, request.IssuingParty);

            var voucher = await _voucherRepository.Get(request.VoucherCode, request.IssuingParty);

            if (voucher is null)
            {
                _logger.LogWarning("Voucher not found with code: {VoucherCode}, issuing party: {IssuingParty}", 
                    request.VoucherCode, request.IssuingParty);
                return new ResourceNotFoundError("Voucher not found");
            }

            var result = voucher.Redeem(request.RedemptionId, request.RedemptionType);
            if (result.IsFailed)
            {
                string errorMessage = result.Errors[0].Message;
                _logger.LogWarning("Voucher cannot be redeemed: {ErrorMessage}", errorMessage);
                
                // Map domain error to application error
                RedeemVoucherError errorCode = errorMessage switch
                {
                    "Voucher is expired" => RedeemVoucherError.VoucherExpired,
                    "Voucher is already redeemed" => RedeemVoucherError.VoucherAlreadyRedeemed,
                    _ => RedeemVoucherError.VoucherCannotBeRedeemed
                };
                
                return new ValidationError(errorMessage, errorCode);
            }

            await _voucherRepository.Update(voucher);

            // Publish message
            try
            {
                await _messagePublishService.SendMessage(_serviceBusSettings.VoucherRedeemTopicName, new VoucherRedeemMessage
                (
                    voucher.Code,
                    voucher.Status.ToString(),
                    voucher.IssuingParty,
                    voucher.Category,
                    voucher.BatchNumber,
                    voucher.RedemptionId,
                    voucher.RedemptionType,
                    voucher.ExpiryDateUtc,
                    voucher.UserDefinedProperties?.Select(udp => new UserDefinedProperty(udp.Id, udp.Values)).ToList()
                ), new Dictionary<string, object> { { Messaging.ServiceBusRedemptionTypePropertyKey, request.RedemptionType } });
            }
            catch (Exception ex)
            {
                // Don't fail the redemption if message publishing fails, just log the error
                _logger.LogError(ex, "Failed to publish voucher redemption message for code: {VoucherCode}", request.VoucherCode);
            }

            _logger.LogInformation("Voucher {VoucherCode} successfully redeemed", request.VoucherCode);
            return Result.Ok(new RedeemVoucherResponse());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error redeeming voucher with code {VoucherCode}", request.VoucherCode);
            return new ValidationError("An unexpected error occurred while redeeming voucher", RedeemVoucherError.UnknownError);
        }
    }
}
