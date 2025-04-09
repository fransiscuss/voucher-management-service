using FluentResults;

namespace Acme.Services.VoucherManagementService.Application.Models.Common;

public class ResourceNotFoundError : Error
{
    public ResourceNotFoundError(string message) : base(message)
    {
    }
}
