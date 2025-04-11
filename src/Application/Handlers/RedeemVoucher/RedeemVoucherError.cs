namespace Acme.Services.VoucherManagementService.Application.Handlers.RedeemVoucher;

public enum RedeemVoucherError
{
    InvalidInput = 3000,
    VoucherNotFound = 3001,
    VoucherCannotBeRedeemed = 3002,
    VoucherExpired = 3003,
    VoucherAlreadyRedeemed = 3004,
    MessagePublishError = 3005,
    UnknownError = 9999
}
