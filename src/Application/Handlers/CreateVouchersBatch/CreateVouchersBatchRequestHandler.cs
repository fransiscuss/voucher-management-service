using System.Collections.Concurrent;
using Acme.Services.VoucherManagementService.Application.Interfaces;
using Acme.Services.VoucherManagementService.Application.Interfaces.Infrastructure;
using Acme.Services.VoucherManagementService.Application.Models.Common;
using Acme.Services.VoucherManagementService.Domain.Entities;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Acme.Services.VoucherManagementService.Application.Handlers.CreateVouchersBatch;

public class CreateVouchersBatchRequestHandler : IRequestHandler<CreateVouchersBatchRequest, Result<CreateVouchersBatchResponse>>
{
    private readonly ICodeGenerationServiceFactory _codeGenerationServiceFactory;
    private readonly IVoucherRepository _voucherRepository;
    private readonly ILogger<CreateVouchersBatchRequestHandler> _logger;

    public CreateVouchersBatchRequestHandler(
        ICodeGenerationServiceFactory codeGenerationServiceFactory,
        IVoucherRepository voucherRepository,
        ILogger<CreateVouchersBatchRequestHandler> logger)
    {
        _codeGenerationServiceFactory = codeGenerationServiceFactory;
        _voucherRepository = voucherRepository;
        _logger = logger;
    }

    public async Task<Result<CreateVouchersBatchResponse>> Handle(CreateVouchersBatchRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var codeGenerationService = _codeGenerationServiceFactory.GetCodeGenerationService(request.CodeGenerationType);

            _logger.LogInformation("Generating {BatchSize} vouchers using {CodeGenerationType}", 
                request.BatchSize, request.CodeGenerationType);
            
            var voucherCodes = await codeGenerationService.Generate(request.BatchSize);

            if (voucherCodes.Count < request.BatchSize)
            {
                _logger.LogWarning("Could only generate {GeneratedCount} of {RequestedCount} voucher codes", 
                    voucherCodes.Count, request.BatchSize);
                return new ValidationError("Could not generate the requested number of unique voucher codes", 
                    CreateVouchersBatchError.CodeGenerationFailed);
            }

            var vouchers = new ConcurrentBag<Voucher>();

            Parallel.ForEach(voucherCodes, (voucherCode) =>
            {
                vouchers.Add(new Voucher(
                    request.IssuingParty,
                    request.Category,
                    request.BatchNumber,
                    voucherCode,
                    VoucherStatus.Active,
                    request.ExpiryDateUtc,
                    request.UserDefinedProperties.ToUserDefinedProperties()));
            });

            _logger.LogInformation("Created {VoucherCount} voucher entities, inserting to database", vouchers.Count);
            var insertedVouchers = await _voucherRepository.BulkInsert(vouchers.ToList());

            _logger.LogInformation("Successfully inserted {InsertedCount} vouchers", insertedVouchers.Count);
            return Result.Ok(new CreateVouchersBatchResponse
            {
                VouchersGenerated = insertedVouchers.Count,
                BatchSize = request.BatchSize
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating voucher batch");
            
            return ex switch
            {
                InvalidOperationException => new ValidationError(ex.Message, CreateVouchersBatchError.CodeGenerationFailed),
                _ => new ValidationError("An unexpected error occurred while creating vouchers", CreateVouchersBatchError.UnknownError)
            };
        }
    }
}
