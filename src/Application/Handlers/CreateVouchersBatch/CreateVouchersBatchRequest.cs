using Acme.Services.VoucherManagementService.Application.Models.Common;
using FluentResults;
using MediatR;

namespace Acme.Services.VoucherManagementService.Application.Handlers.CreateVouchersBatch;

public class CreateVouchersBatchRequest : IRequest<Result<CreateVouchersBatchResponse>>
{
    public required string IssuingParty { get; set; }
    public required string Category { get; set; }
    public required string BatchNumber { get; set; }
    public int BatchSize { get; set; } = 1;
    public DateTime ExpiryDateUtc { get; set; }
    public string CodeGenerationType { get; set; } = CodeGenerationType.UniqueCode;
    public List<UserDefinedProperty>? UserDefinedProperties { get; set; }
}
