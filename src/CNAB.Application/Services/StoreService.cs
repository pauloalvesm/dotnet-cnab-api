using CNAB.Application.DTOs;
using CNAB.Application.Interfaces;
using CNAB.Domain.Entities;
using CNAB.Domain.Interfaces.Repositories;
using MapsterMapper;
using Microsoft.Extensions.Logging;

namespace CNAB.Application.Services;

public class StoreService : IStoreService
{
    private readonly IStoreRepository _storeRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<StoreService> _logger;

    public StoreService(IStoreRepository storeRepository, IMapper mapper, ILogger<StoreService> logger)
    {
        _storeRepository = storeRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<StoreDto>> GetAllStoreAsync()
    {
        var stores = await _storeRepository.GetAllStores();

        return stores.Select(store => new StoreDto
        {
            Id = store.Id,
            Name = store.Name,
            OwnerName = store.OwnerName,
            Balance = store.GetBalance()
        });
    }

    public async Task<StoreDto> GetByIdStoreAsync(Guid id)
    {
        var store = await _storeRepository.GetStoreById(id);

        if (store == null)
        {
            _logger.LogError("Store not found");
            return null;
        }

        return new StoreDto
        {
            Id = store.Id,
            Name = store.Name,
            OwnerName = store.OwnerName,
            Balance = store.GetBalance()
        };
    }

    public async Task<StoreDto> AddStoreAsync(StoreInputDto storeInputDto)
    {

        var store = new Store(storeInputDto.Name, storeInputDto.OwnerName);

        await _storeRepository.AddStore(store);

        return _mapper.Map<StoreDto>(store);
    }

    public async Task<StoreDto> UpdateStoreAsync(StoreInputDto storeInputDto)
    {

        var existingStore = await _storeRepository.GetStoreById(storeInputDto.Id);

        if (existingStore == null)
        {
            _logger.LogError("Store not found");
            return null;
        }

        existingStore.UpdateDetails(storeInputDto.Name, storeInputDto.OwnerName);

        await _storeRepository.UpdateStore(existingStore);

        return _mapper.Map<StoreDto>(existingStore);
    }

    public async Task DeleteStoreAsync(Guid id)
    {
        var existingStore = await _storeRepository.GetStoreById(id);

        if (existingStore == null)
        {
            _logger.LogError("Store not found");
        }

        await _storeRepository.DeleteStore(id);
    }

    public async Task<decimal> GetStoreBalanceAsync(Guid storeId)
    {
        var existingStore = await _storeRepository.GetStoreById(storeId);

        if (existingStore == null)
        {
            _logger.LogError("Store not found");
            return 0m;
        }

        return existingStore.GetBalance();
    }
}