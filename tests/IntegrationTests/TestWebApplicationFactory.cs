using Acme.Services.VoucherManagementService.Api;
using Acme.Services.VoucherManagementService.Application.Interfaces.Infrastructure;
using Acme.Services.VoucherManagementService.Infrastructure.Settings;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Moq;
using System.Net.Http.Headers;
using Testcontainers.CosmosDb;

namespace Acme.Services.VoucherManagementService.IntegrationTests;

public class TestWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly CosmosDbContainer _cosmosDbContainer;
    private readonly Mock<IMessagePublishService> _mockMessagePublishService;
    private readonly string _apiKey = "test-api-key";

    public TestWebApplicationFactory()
    {
        _cosmosDbContainer = new CosmosDbBuilder()
            .WithImage("mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator:latest")
            .WithPortBinding(8081, 8081)
            .WithExposedPort(8081)
            .Build();

        _mockMessagePublishService = new Mock<IMessagePublishService>();
        _mockMessagePublishService.Setup(m => m.SendMessage(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<IDictionary<string, object>>()))
            .Returns(Task.CompletedTask);
    }

    public HttpClient GetAuthenticatedClient()
    {
        var client = CreateClient();
        client.DefaultRequestHeaders.Add("X-API-Key", _apiKey);
        return client;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(config =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["Authentication:ApiKey"] = _apiKey,
                ["CosmosDb:DatabaseId"] = "voucher-service-test",
                ["CosmosDb:VoucherContainerId"] = "vouchers",
                ["CosmosDb:DocumentEndpoint"] = _cosmosDbContainer.GetConnectionString(),
                ["CosmosDb:AuthKey"] = _cosmosDbContainer.GetContainerPassword(),
                ["CosmosDb:BulkVoucherInsertRetryAttempts"] = "3",
                ["ServiceBus:VoucherRedeemTopicName"] = "voucher-redeemed-test",
                ["ShortCode:EnableShortCodeGeneration"] = "true",
                ["ShortCode:Length"] = "6",
                ["ShortCode:CharacterSet"] = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789"
            });
        });

        builder.ConfigureTestServices(services =>
        {
            // Replace the real message service with a mock
            services.RemoveAll<IMessagePublishService>();
            services.AddSingleton(_mockMessagePublishService.Object);

            // Configure CosmosDB client
            services.RemoveAll<CosmosClient>();
            services.AddSingleton(sp =>
            {
                var cosmosSettings = sp.GetRequiredService<IOptionsMonitor<CosmosDbSetting>>().CurrentValue;
                return new CosmosClient(
                    cosmosSettings.DocumentEndpoint,
                    cosmosSettings.AuthKey,
                    new CosmosClientOptions
                    {
                        SerializerOptions = new CosmosSerializationOptions
                        {
                            PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
                        },
                        ConnectionMode = ConnectionMode.Gateway
                    });
            });
        });
    }

    public async Task InitializeAsync()
    {
        await _cosmosDbContainer.StartAsync();

        // Initialize database and container
        var client = CreateClient();

        // Initialize Cosmos DB
        var cosmosClient = new CosmosClient(
            _cosmosDbContainer.GetConnectionString(),
            _cosmosDbContainer.GetContainerPassword(),
            new CosmosClientOptions
            {
                ConnectionMode = ConnectionMode.Gateway
            });

        var databaseResponse = await cosmosClient.CreateDatabaseIfNotExistsAsync("voucher-service-test");
        await databaseResponse.Database.CreateContainerIfNotExistsAsync("vouchers", "/id");
    }

    public new async Task DisposeAsync()
    {
        await _cosmosDbContainer.DisposeAsync();
    }
}
