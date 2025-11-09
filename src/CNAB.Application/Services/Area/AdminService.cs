using CNAB.Application.Interfaces;
using CNAB.Application.Interfaces.Area;

namespace CNAB.Application.Services.Area;

public class AdminService : IAdminService
{
    private readonly IStoreService _storeService;
    private readonly ITransactionService _transactionService;

    public AdminService(IStoreService storeService, ITransactionService transactionService)
    {
        _storeService = storeService;
        _transactionService = transactionService;
    }
    
    public async Task<decimal> GetTotalBalanceAsync()
    {
        var stores = await _storeService.GetAllStoreAsync();
        return stores.Sum(s => s.Balance);
    }

    public async Task<int> GetStoreCountAsync()
    {
        var stores = await _storeService.GetAllStoreAsync();
        return stores.Count();
    }

    public async Task<int> GetTransactionCountAsync()
    {
        var transactions = await _transactionService.GetAllTransactionsAsync();
        return transactions.Count();
    }
}