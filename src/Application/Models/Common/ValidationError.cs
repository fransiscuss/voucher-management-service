using FluentResults;

namespace Acme.Services.VoucherManagementService.Application.Models.Common;

public class ValidationError : Error
{
    public ValidationError(string message, Enum errorCode) : base(message)
    {
        Metadata.Add("ErrorCode", errorCode);
    }
}
