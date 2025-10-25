using CNAB.Domain.Entities;

namespace CNAB.Domain.Interfaces.Repositories;

public interface IStoreRepository
{
    Task<IEnumerable<Store>> GetAllStores();
    Task<Store> GetStoreById(Guid id);
    Task<Store> GetStoreByName(string storeName);
    Task<Store> AddStore(Store Store);
    Task<Store> UpdateStore(Store Store);
    Task DeleteStore(Guid id);
}