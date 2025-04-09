namespace Acme.Services.VoucherManagementService.Application.Interfaces.Infrastructure;

public interface IMessagePublishService
{
    Task SendMessage<T>(string topicName, T message, IDictionary<string, object>? applicationProperties = null);
}
