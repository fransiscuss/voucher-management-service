using Acme.Services.VoucherManagementService.Domain.Entities;

namespace Acme.Services.VoucherManagementService.Application.Interfaces.Infrastructure;

public interface IVoucherRepository
{
    Task<Voucher> Add(Voucher voucher);
    Task<List<Voucher>> BulkInsert(List<Voucher> vouchers);
    Task<Voucher?> Get(string voucherCode, string issuingParty);
    Task<List<Voucher>> Get(string issuingParty, string batchNumber, List<VoucherStatus> statuses);
    Task<Voucher?> GetByVoucherCode(string voucherCode);
    Task<List<Voucher>> GetVouchers(List<string> voucherCodes);
    Task Update(Voucher voucher);
}
