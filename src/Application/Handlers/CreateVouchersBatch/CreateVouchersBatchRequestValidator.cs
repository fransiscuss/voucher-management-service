using Acme.Services.VoucherManagementService.Application.Common.Extensions;
using FluentValidation;

namespace Acme.Services.VoucherManagementService.Application.Handlers.CreateVouchersBatch;

public class CreateVouchersBatchRequestValidator : AbstractValidator<CreateVouchersBatchRequest>
{
    public CreateVouchersBatchRequestValidator()
    {
        RuleFor(x => x.IssuingParty)
            .NotEmpty()
            .WithMessage("Issuing party is required")
            .WithErrorCode(CreateVouchersBatchError.InvalidInput);

        RuleFor(x => x.Category)
            .NotEmpty()
            .WithMessage("Category is required")
            .WithErrorCode(CreateVouchersBatchError.InvalidInput);

        RuleFor(x => x.BatchNumber)
            .NotEmpty()
            .WithMessage("Batch number is required")
            .WithErrorCode(CreateVouchersBatchError.InvalidInput);

        RuleFor(x => x.BatchSize)
            .GreaterThan(0)
            .WithMessage("Batch size must be greater than 0")
            .WithErrorCode(CreateVouchersBatchError.InvalidInput)
            .LessThanOrEqualTo(1000)
            .WithMessage("Batch size must not exceed 1000")
            .WithErrorCode(CreateVouchersBatchError.InvalidInput);

        RuleFor(x => x.ExpiryDateUtc)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("Expiry date must be in the future")
            .WithErrorCode(CreateVouchersBatchError.InvalidInput);

        RuleFor(x => x.CodeGenerationType)
            .Must(type => type == CodeGenerationType.UniqueCode || type == CodeGenerationType.ShortCode)
            .WithMessage("Code generation type must be either 'UniqueCode' or 'ShortCode'")
            .WithErrorCode(CreateVouchersBatchError.InvalidCodeGenerationType);

        When(x => x.UserDefinedProperties != null && x.UserDefinedProperties.Any(), () =>
        {
            RuleForEach(x => x.UserDefinedProperties)
                .ChildRules(properties =>
                {
                    properties.RuleFor(p => p.Id)
                        .NotEmpty()
                        .WithMessage("User defined property ID is required")
                        .WithErrorCode(CreateVouchersBatchError.InvalidInput);

                    properties.RuleFor(p => p.Values)
                        .NotNull()
                        .WithMessage("User defined property values cannot be null")
                        .WithErrorCode(CreateVouchersBatchError.InvalidInput);
                });
        });
    }
}
