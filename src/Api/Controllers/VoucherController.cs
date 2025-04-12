using System.ComponentModel.DataAnnotations;
using Acme.Services.VoucherManagementService.Api.Extensions;
using Acme.Services.VoucherManagementService.Application.Handlers.CreateVouchersBatch;
using Acme.Services.VoucherManagementService.Application.Handlers.GetVoucher;
using Acme.Services.VoucherManagementService.Application.Handlers.RedeemVoucher;
using Acme.Services.VoucherManagementService.Application.Handlers.ValidateVoucher;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Acme.Services.VoucherManagementService.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/vouchers")]
public class VoucherController : ControllerBase
{
    private readonly IMediator _mediator;

    public VoucherController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("batch")]
    [SwaggerOperation(Summary = "Create a batch of vouchers", OperationId = nameof(CreateVouchers))]
    [ProducesResponseType(typeof(CreateVouchersBatchResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails<CreateVouchersBatchError>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateVouchers(CreateVouchersBatchRequest request)
    {
        return await _mediator.SendWithErrorHandling<CreateVouchersBatchResponse, CreateVouchersBatchError>(
            request, System.Net.HttpStatusCode.Created);
    }

    [HttpPost("{voucherCode}/redeem")]
    [SwaggerOperation(Summary = "Redeem a voucher", OperationId = nameof(Redeem))]
    [ProducesResponseType(typeof(ProblemDetails<RedeemVoucherError>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Redeem(
        RedeemVoucherRequest request,
        string voucherCode,
        [Required][FromHeader] string issuingParty
    )
    {
        request.VoucherCode = voucherCode;
        request.IssuingParty = issuingParty;

        return await _mediator.SendWithErrorHandling<RedeemVoucherResponse, RedeemVoucherError>(
            request, System.Net.HttpStatusCode.NoContent);
    }

    [HttpPost("{voucherCode}/validate")]
    [SwaggerOperation(Summary = "Validate a voucher", OperationId = nameof(Validate))]
    [ProducesResponseType(typeof(ValidateVoucherResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails<ValidateVoucherError>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Validate(string voucherCode, [Required][FromHeader] string issuingParty)
    {
        return await _mediator.SendWithErrorHandling<ValidateVoucherResponse, ValidateVoucherError>(
            new ValidateVoucherRequest
            {
                IssuingParty = issuingParty,
                VoucherCode = voucherCode
            });
    }

    [HttpGet("{voucherCode}")]
    [SwaggerOperation(Summary = "Get a voucher", OperationId = nameof(Get))]
    [ProducesResponseType(typeof(GetVoucherResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails<GetVoucherError>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Get(string voucherCode, [FromHeader] string? issuingParty = null)
    {
        var request = new GetVoucherRequest
        {
            VoucherCode = voucherCode,
            IssuingParty = issuingParty
        };

        return await _mediator.SendWithErrorHandling<GetVoucherResponse, GetVoucherError>(request);
    }
}
