using CNAB.Application.DTOs;

namespace CNAB.Application.Interfaces;

public interface IStoreService
{
    Task<IEnumerable<StoreDto>> GetAllStoreAsync();
    Task<StoreDto> GetByIdStoreAsync(Guid id);
    Task<StoreDto> AddStoreAsync(StoreInputDto storeInputDto);
    Task<StoreDto> UpdateStoreAsync(StoreInputDto storeInputDto);
    Task DeleteStoreAsync(Guid id);
    Task<decimal> GetStoreBalanceAsync(Guid storeId);
}