using CNAB.Application.DTOs;
using CNAB.Application.Services;
using CNAB.Domain.Entities;
using CNAB.Domain.Entities.enums;
using CNAB.Domain.Interfaces.Repositories;
using CNAB.TestHelpers.Factories;
using MapsterMapper;
using Microsoft.Extensions.Logging;
using Moq;

namespace CNAB.Application.Test.Services;

public class StoreServiceTest
{
    private readonly Mock<IStoreRepository> _mockStoreRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILogger<StoreService>> _mockLogger;
    private readonly StoreService _storeService;

    public StoreServiceTest()
    {
        _mockStoreRepository = new Mock<IStoreRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILogger<StoreService>>();
        _storeService = new StoreService(_mockStoreRepository.Object, _mockMapper.Object, _mockLogger.Object);
    }

    [Fact(DisplayName = "GetAllStoreAsync - Should return all Stores")]
    public async Task StoreService_GetAllStoreAsync_ShouldReturnAllStores()
    {
        // Arrange
        var stores = ServiceTestFactory.GenerateMockStores();
        stores[0].AddTransaction(new Transaction(TransactionType.Credit, DateTime.Now, 500m, "12345678901", "1234", TimeSpan.Zero, stores[0]));
        stores[1].AddTransaction(new Transaction(TransactionType.Debit, DateTime.Now, 200m, "12345678901", "1234", TimeSpan.Zero, stores[1]));
        _mockStoreRepository.Setup(repo => repo.GetAllStores()).ReturnsAsync(stores);

        // Act
        var result = await _storeService.GetAllStoreAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(stores.Count, result.Count());
        Assert.Equal(stores[0].Name, result.ElementAt(0).Name);
        Assert.Equal(stores[0].GetBalance(), result.ElementAt(0).Balance);
    }

    [Fact(DisplayName = "GetAllStoreAsync - Should return empty list when no Stores exist")]
    public async Task StoreService_GetAllStoreAsync_ShouldReturnEmptyListWhenNoStoresExist()
    {
        // Arrange
        _mockStoreRepository.Setup(repo => repo.GetAllStores()).ReturnsAsync(new List<Store>());

        // Act
        var result = await _storeService.GetAllStoreAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact(DisplayName = "GetByIdStoreAsync - Should return StoreDto when id exists")]
    public async Task StoreService_GetByIdStoreAsync_ShouldReturnStoreDtoWhenIdExists()
    {
        // Arrange
        var storeId = Guid.NewGuid();
        var store = new Store(storeId, "Store D", "Owner D");
        store.AddTransaction(new Transaction(TransactionType.Credit, DateTime.Now, 100m, "12345678901", "1234", TimeSpan.Zero, store));

        _mockStoreRepository.Setup(repo => repo.GetStoreById(storeId)).ReturnsAsync(store);

        // Act
        var result = await _storeService.GetByIdStoreAsync(storeId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(storeId, result.Id);
        Assert.Equal(store.Name, result.Name);
        Assert.Equal(store.OwnerName, result.OwnerName);
        Assert.Equal(store.GetBalance(), result.Balance);
    }

    [Fact(DisplayName = "GetByIdStoreAsync - Should return null when StoreDto Id does not exist")]
    public async Task StoreService_GetByIdStoreAsync_ShouldReturnNullWhenStoreDtoIdDoesNotExist()
    {
        // Arrange
        var invalidStoreId = Guid.NewGuid();
        _mockStoreRepository.Setup(repo => repo.GetStoreById(invalidStoreId)).ReturnsAsync((Store)null);

        // Act
        var result = await _storeService.GetByIdStoreAsync(invalidStoreId);

        // Assert
        Assert.Null(result);

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString().Contains("Store not found")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Fact(DisplayName = "AddStoreAsync - Should insert StoreInputDto and return StoreDto")]
    public async Task StoreService_AddStoreAsync_ShouldInsertStoreInputDtoAndReturnStoreDto()
    {
        // Arrange
        var storeInputDto = ServiceTestFactory.CreateStoreInputDto();
        var expectedStore = new Store(storeInputDto.Name, storeInputDto.OwnerName);
        var expectedStoreDto = ServiceTestFactory.CreateStoreDto(expectedStore.Id, expectedStore.Name, expectedStore.OwnerName);
        _mockMapper.Setup(mapper => mapper.Map<StoreDto>(It.IsAny<Store>())).Returns(expectedStoreDto);

        // Act
        var result = await _storeService.AddStoreAsync(storeInputDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedStoreDto.Name, result.Name);
        Assert.Equal(expectedStoreDto.OwnerName, result.OwnerName);

        _mockStoreRepository.Verify(repo => repo.AddStore(It.Is<Store>(s =>
            s.Name == storeInputDto.Name &&
            s.OwnerName == storeInputDto.OwnerName)),
            Times.Once);
    }

    [Fact(DisplayName = "AddStoreAsync - Should return null when mapper returns null")]
    public async Task StoreService_AddStoreAsync_ShouldReturnNullWhenMapperReturnsNull()
    {
        // Arrange
        var storeInputDto = ServiceTestFactory.CreateStoreInputDto();
        _mockMapper.Setup(mapper => mapper.Map<StoreDto>(It.IsAny<Store>())).Returns((StoreDto)null);

        // Act
        var result = await _storeService.AddStoreAsync(storeInputDto);

        // Assert
        Assert.Null(result);

        _mockStoreRepository.Verify(repo => repo.AddStore(It.IsAny<Store>()), Times.Once);
    }

    [Fact(DisplayName = "UpdateStoreAsync - Should update and return updated StoreDto")]
    public async Task StoreService_UpdateStoreAsync_ShouldUpdateAndReturnUpdatedStoreDto()
    {
        // Arrange
        var storeId = Guid.NewGuid();
        var existingStore = new Store(storeId, "Original Store", "Original Owner");
        var storeInputDto = ServiceTestFactory.UpdateStoreInputDto(storeId);
        var expectedStoreDto = ServiceTestFactory.CreateStoreDto(storeId, storeInputDto.Name, storeInputDto.OwnerName);
        _mockStoreRepository.Setup(repo => repo.GetStoreById(storeId)).ReturnsAsync(existingStore);
        _mockMapper.Setup(mapper => mapper.Map<StoreDto>(It.IsAny<Store>())).Returns(expectedStoreDto);

        // Act
        var result = await _storeService.UpdateStoreAsync(storeInputDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedStoreDto.Id, result.Id);
        Assert.Equal(expectedStoreDto.Name, result.Name);
        Assert.Equal(expectedStoreDto.OwnerName, result.OwnerName);

        _mockStoreRepository.Verify(repo => repo.UpdateStore(It.Is<Store>(s =>
            s.Id == storeId &&
            s.Name == storeInputDto.Name &&
            s.OwnerName == storeInputDto.OwnerName)),
            Times.Once);
    }

    [Fact(DisplayName = "UpdateStoreAsync - Should return null when Store not found")]
    public async Task StoreService_UpdateStoreAsync_ShouldReturnNullWhenStoreNotFound()
    {
        // Arrange
        var storeId = Guid.NewGuid();
        var storeInputDto = ServiceTestFactory.UpdateStoreInputDto(storeId);
        _mockStoreRepository.Setup(repo => repo.GetStoreById(storeId)).ReturnsAsync((Store)null);

        // Act
        var result = await _storeService.UpdateStoreAsync(storeInputDto);

        // Assert
        Assert.Null(result);

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString().Contains("Store not found")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);

        _mockStoreRepository.Verify(repo => repo.UpdateStore(It.IsAny<Store>()), Times.Never);
    }

    [Fact(DisplayName = "DeleteStoreAsync - Should delete Store when exists")]
    public async Task StoreService_DeleteStoreAsync_ShouldDeleteStoreWhenExists()
    {
        // Arrange
        var storeId = Guid.NewGuid();
        var existingStore = new Store(storeId, "Store to Delete", "Owner");
        _mockStoreRepository.Setup(repo => repo.GetStoreById(storeId)).ReturnsAsync(existingStore);

        // Act
        await _storeService.DeleteStoreAsync(storeId);

        // Assert
        _mockStoreRepository.Verify(repo => repo.DeleteStore(storeId), Times.Once);
    }

    [Fact(DisplayName = "DeleteStoreAsync - Should log error when Store not found")]
    public async Task StoreService_DeleteStoreAsync_ShouldLogErrorWhenStoreNotFound()
    {
        // Arrange
        var storeId = Guid.NewGuid();
        _mockStoreRepository.Setup(repo => repo.GetStoreById(storeId)).ReturnsAsync((Store)null);

        // Act
        await _storeService.DeleteStoreAsync(storeId);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString().Contains("Store not found")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);

        _mockStoreRepository.Verify(repo => repo.DeleteStore(storeId), Times.Once);
    }

    [Fact(DisplayName = "GetStoreBalanceAsync - Should return correct balance when Store exists")]
    public async Task StoreService_GetStoreBalanceAsync_ShouldReturnCorrectBalanceWhenStoreExists()
    {
        // Arrange
        var storeId = Guid.NewGuid();
        var store = new Store(storeId, "Test Store", "Test Owner");
        store.AddTransaction(new Transaction(TransactionType.Credit, DateTime.Now, 150m, "12345678901", "1234", TimeSpan.Zero, store));
        store.AddTransaction(new Transaction(TransactionType.Debit, DateTime.Now, 50m, "12345678901", "1234", TimeSpan.Zero, store));
        _mockStoreRepository.Setup(repo => repo.GetStoreById(storeId)).ReturnsAsync(store);

        // Act
        var expected = 200m;
        var result = await _storeService.GetStoreBalanceAsync(storeId);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact(DisplayName = "GetStoreBalanceAsync - Should return zero when Store does not exist")]
    public async Task StoreService_GetStoreBalanceAsync_ShouldReturnZeroWhenStoreDoesNotExist()
    {
        // Arrange
        var storeId = Guid.NewGuid();
        _mockStoreRepository.Setup(repo => repo.GetStoreById(storeId)).ReturnsAsync((Store)null);

        // Act
        var expected = 0m;
        var result = await _storeService.GetStoreBalanceAsync(storeId);

        // Assert
        Assert.Equal(expected, result);
    }
}