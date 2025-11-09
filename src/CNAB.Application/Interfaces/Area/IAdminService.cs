namespace CNAB.Application.Interfaces.Area;

public interface IAdminService
{
    Task<decimal> GetTotalBalanceAsync();
    Task<int> GetStoreCountAsync();
    Task<int> GetTransactionCountAsync();
}