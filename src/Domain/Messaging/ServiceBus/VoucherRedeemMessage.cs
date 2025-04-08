namespace Acme.Services.VoucherManagementService.Domain.Messaging.ServiceBus;

public record UserDefinedProperty(string Id, List<string> Values);

public record VoucherRedeemMessage(
    string Code,
    string Status,
    string IssuingParty,
    string Category,
    string BatchNumber,
    string? RedemptionId,
    string? RedemptionType,
    DateTime ExpiryDateUtc,
    List<UserDefinedProperty>? UserDefinedProperties
);
