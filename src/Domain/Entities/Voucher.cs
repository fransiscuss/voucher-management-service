using FluentResults;

namespace Acme.Services.VoucherManagementService.Domain.Entities;

public class Voucher : Entity
{
    private VoucherStatus _status;

    public VoucherStatus Status
    {
        get
        {
            if (ExpiryDateUtc < DateTime.UtcNow && _status != VoucherStatus.Redeemed)
            {
                return VoucherStatus.Expired;
            }
            else { return _status; }
        }
        private set { _status = value; }
    }

    public string IssuingParty { get; private set; }
    public string Category { get; private set; }
    public DateTime ExpiryDateUtc { get; private set; }
    public string BatchNumber { get; private set; }
    public string Code { get; private set; }
    public string? RedemptionId { get; private set; }
    public string? RedemptionType { get; private set; }
    public DateTime? RedeemedDateUtc { get; private set; }
    public DateTime CreatedDateUtc { get; set; }

    public List<UserDefinedProperty>? UserDefinedProperties { get; private set; }

    public Result Redeem(string redemptionId, string redemptionType)
    {
        var validateResult = Validate();

        if (validateResult.IsFailed)
        {
            return validateResult;
        }

        Status = VoucherStatus.Redeemed;
        RedemptionId = redemptionId;
        RedemptionType = redemptionType;
        RedeemedDateUtc = DateTime.UtcNow;

        return Result.Ok();
    }

    public Result Validate()
    {
        if (Status != VoucherStatus.Active)
        {
            if (Status == VoucherStatus.Expired)
            {
                return new Error("Voucher is expired");
            }

            if (Status == VoucherStatus.Redeemed)
            {
                return new Error("Voucher is already redeemed");
            }

            return new Error("Voucher is not active");
        }

        return Result.Ok();
    }

    public Voucher(
        string issuingParty,
        string category,
        string batchNumber,
        string code,
        VoucherStatus status,
        DateTime expiryDateUtc,
        List<UserDefinedProperty>? userDefinedProperties
    )
    {
        Id = Guid.NewGuid().ToString();
        IssuingParty = issuingParty;
        Category = category;
        BatchNumber = batchNumber;
        Code = code;
        _status = status;
        ExpiryDateUtc = expiryDateUtc.Date;
        CreatedDateUtc = DateTime.UtcNow;
        UserDefinedProperties = userDefinedProperties;
    }
}
