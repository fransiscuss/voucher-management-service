using Acme.Services.VoucherManagementService.Application.Interfaces.Infrastructure;
using Acme.Services.VoucherManagementService.Domain.Entities;
using Acme.Services.VoucherManagementService.Infrastructure.Settings;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;

namespace Acme.Services.VoucherManagementService.Infrastructure.Persistence;

public class VoucherRepository : IVoucherRepository
{
    private readonly CosmosDbSetting _setting;
    private readonly CosmosClient _cosmosClient;
    private readonly ILogger<VoucherRepository> _logger;

    public VoucherRepository(
        IOptionsMonitor<CosmosDbSetting> cosmosDbSettings, 
        CosmosClient cosmosClient, 
        ILogger<VoucherRepository> logger)
    {
        _setting = cosmosDbSettings.CurrentValue;
        _cosmosClient = cosmosClient;
        _logger = logger;
    }

    public async Task<Voucher> Add(Voucher voucher)
    {
        var container = _cosmosClient.GetContainer(_setting.DatabaseId, _setting.VoucherContainerId);
        var response = await container.CreateItemAsync(voucher, new PartitionKey(voucher.Id));
        return response.Resource;
    }

    public async Task<List<Voucher>> BulkInsert(List<Voucher> vouchers)
    {
        var insertedVouchers = new List<Voucher>();

        await Policy
                .HandleResult<List<Voucher>>(faultedVouchers => faultedVouchers.Any())
                .RetryAsync(_setting.BulkVoucherInsertRetryAttempts)
                .ExecuteAsync(async () =>
                {
                    var result = await ExecuteBulkInsert(vouchers);

                    if (result.Any())
                    {
                        insertedVouchers.AddRange(result);
                    }

                    if (result.Count != vouchers.Count)
                    {
                        var faultedVouchers = vouchers.Where(t => !result.Any(it => it.Id == t.Id)).ToList();
                        
                        // Only retry with the vouchers that failed to insert
                        vouchers = faultedVouchers;
                        return faultedVouchers;
                    }
                    
                    return new List<Voucher>();
                });

        return insertedVouchers;
    }

    public async Task<Voucher?> Get(string voucherCode, string issuingParty)
    {
        var container = _cosmosClient.GetContainer(_setting.DatabaseId, _setting.VoucherContainerId);

        var query = container
            .GetItemLinqQueryable<Voucher>()
            .Where(c => c.Code.ToUpper() == voucherCode.ToUpper() && c.IssuingParty == issuingParty);

        using var feedIterator = query.ToFeedIterator();

        while (feedIterator.HasMoreResults)
        {
            foreach (var voucher in await feedIterator.ReadNextAsync())
            {
                return voucher;
            }
        }

        return null;
    }

    public async Task<List<Voucher>> Get(string issuingParty, string batchNumber, List<VoucherStatus> statuses)
    {
        var container = _cosmosClient.GetContainer(_setting.DatabaseId, _setting.VoucherContainerId);

        var query = container
            .GetItemLinqQueryable<Voucher>()
            .Where(c => c.BatchNumber == batchNumber &&
                        c.IssuingParty == issuingParty);

        using var feedIterator = query.ToFeedIterator();

        var vouchers = new List<Voucher>();

        while (feedIterator.HasMoreResults)
        {
            foreach (var voucher in await feedIterator.ReadNextAsync())
            {
                if (statuses.Contains(voucher.Status))
                {
                    vouchers.Add(voucher);
                }
            }
        }

        return vouchers;
    }

    public async Task<Voucher?> GetByVoucherCode(string voucherCode)
    {
        var container = _cosmosClient.GetContainer(_setting.DatabaseId, _setting.VoucherContainerId);

        var query = container.GetItemLinqQueryable<Voucher>().Where(c => c.Code.ToUpper() == voucherCode.ToUpper());

        using var feedIterator = query.ToFeedIterator();

        while (feedIterator.HasMoreResults)
        {
            foreach (var voucher in await feedIterator.ReadNextAsync())
            {
                return voucher;
            }
        }

        return null;
    }

    public async Task<List<Voucher>> GetVouchers(List<string> voucherCodes)
    {
        var vouchers = new List<Voucher>();
        var container = _cosmosClient.GetContainer(_setting.DatabaseId, _setting.VoucherContainerId);

        // Normalize all codes to uppercase for case-insensitive comparison
        var normalizedCodes = voucherCodes.Select(c => c.ToUpper()).ToList();
        
        var query = container.GetItemLinqQueryable<Voucher>()
            .Where(c => normalizedCodes.Contains(c.Code.ToUpper()));

        using var feedIterator = query.ToFeedIterator();

        while (feedIterator.HasMoreResults)
        {
            vouchers.AddRange(await feedIterator.ReadNextAsync());
        }

        return vouchers;
    }

    public async Task Update(Voucher voucher)
    {
        var container = _cosmosClient.GetContainer(_setting.DatabaseId, _setting.VoucherContainerId);
        await container.ReplaceItemAsync(voucher, voucher.Id, new PartitionKey(voucher.Id));
    }

    private async Task<List<Voucher>> ExecuteBulkInsert(List<Voucher> vouchers)
    {
        var container = _cosmosClient.GetContainer(_setting.DatabaseId, _setting.VoucherContainerId);
        var tasks = new List<Task<ItemResponse<Voucher>>>();

        try
        {
            foreach (var voucher in vouchers)
            {
                tasks.Add(container.CreateItemAsync(voucher, new PartitionKey(voucher.Id)));
            }

            await Task.WhenAll(tasks);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error bulk inserting vouchers");
        }

        return tasks.Where(t => t.IsCompletedSuccessfully).Select(t => t.Result.Resource).ToList();
    }
}
