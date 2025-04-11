namespace Acme.Services.VoucherManagementService.Application.Handlers.CreateVouchersBatch;

public enum CreateVouchersBatchError
{
    InvalidInput = 1000,
    InvalidCodeGenerationType = 1001,
    CodeGenerationFailed = 1002,
    DatabaseError = 1003,
    UnknownError = 9999
}
