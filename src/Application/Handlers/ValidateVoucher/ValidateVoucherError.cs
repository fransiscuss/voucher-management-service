namespace Acme.Services.VoucherManagementService.Application.Handlers.ValidateVoucher;

public enum ValidateVoucherError
{
    InvalidInput = 4000,
    VoucherNotFound = 4001,
    VoucherExpired = 4002,
    VoucherAlreadyRedeemed = 4003,
    VoucherNotActive = 4004,
    UnknownError = 9999
}
