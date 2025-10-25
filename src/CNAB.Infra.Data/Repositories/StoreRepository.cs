using CNAB.Domain.Entities;
using CNAB.Domain.Interfaces.Repositories;
using CNAB.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CNAB.Infra.Data.Repositories;

public class StoreRepository : IStoreRepository
{
    protected readonly ApplicationDbContext _storeContext;
    protected readonly ILogger<TransactionRepository> _logger;

    public StoreRepository(ApplicationDbContext storeContext, ILogger<TransactionRepository> logger)
    {
        _storeContext = storeContext;
        _logger = logger;
    }

    public async Task<IEnumerable<Store>> GetAllStores()
    {
        var stores = await _storeContext.Set<Store>()
            .Include(s => s.Transactions)
            .ToListAsync();

        if (stores == null || !stores.Any())
        {
            _logger.LogWarning("No stores found in the database.");
            return Enumerable.Empty<Store>();
        }

        return stores;
    }

    public async Task<Store> GetStoreById(Guid id)
    {
        var store = await _storeContext.Set<Store>()
            .Include(s => s.Transactions)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (store == null)
        {
            _logger.LogWarning($"No store found with ID: {id}");
        }

        return store;
    }

    public async Task<Store> GetStoreByName(string storeName)
    {
        var store = await _storeContext.Stores.FirstOrDefaultAsync(s => s.Name == storeName);

        if (store == null)
        {
            _logger.LogWarning($"No store found with name: {storeName}");
        }

        return store;
    }

    public async Task<Store> AddStore(Store store)
    {
        try
        {
            _storeContext.Set<Store>().Add(store);
            await _storeContext.SaveChangesAsync();
            return store;
        }
        catch (DbUpdateException exception)
        {
            _logger.LogError($"Error when adding a new store: {exception.Message}");
            throw;
        }
    }

    public async Task<Store> UpdateStore(Store store)
    {
        try
        {
            _storeContext.Update(store);
            await _storeContext.SaveChangesAsync();
            return store;
        }
        catch (DbUpdateException exception)
        {
            _logger.LogError($"Error when updating the store: {exception.Message}");
            throw;
        }
    }

    public async Task DeleteStore(Guid id)
    {
        try
        {
            var store = await _storeContext.Set<Store>().FindAsync(id);

            if (store != null)
            {
                _storeContext.Set<Store>().Remove(store);
                await _storeContext.SaveChangesAsync();
            }
        }
        catch (DbUpdateException exception)
        {
            _logger.LogError($"Error when deleting the store: {exception.Message}");
            throw;
        }
    }
}