using CNAB.Infra.Data.Context;
using CNAB.Infra.Data.Repositories;
using CNAB.Infra.Data.Test.Common;
using CNAB.TestHelpers.Factories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace CNAB.Infra.Data.Test.Repositories;

public class StoreRepositoryTest
{
    private readonly Mock<ApplicationDbContext> _mockContext;
    private readonly Mock<ILogger<TransactionRepository>> _mockLogger;
    private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;

    public StoreRepositoryTest()
    {
        _mockLogger = new Mock<ILogger<TransactionRepository>>();

        _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _mockContext = new Mock<ApplicationDbContext>(_dbContextOptions) { CallBase = true };
    }

    [Fact(DisplayName = "GetAllStores - Should return all stores")]
    public async Task StoreRepository_GetAllStores_ShouldReturnAllStores()
    {
        // Arrange
        using var context = new ApplicationDbContext(_dbContextOptions);
        var stores = RepositoryTestFactory.GenerateListStores();

        context.Stores.AddRange(stores);
        await context.SaveChangesAsync();

        var repository = new StoreRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetAllStores();

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().HaveCount(stores.Count);
    }

    [Fact(DisplayName = "GetAllStores - Should return empty list when no stores")]
    public async Task StoreRepository_GetAllStores_ShouldReturnEmptyListWhenNoStores()
    {
        // Arrange
        using var context = new ApplicationDbContext(_dbContextOptions);
        var repository = new StoreRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetAllStores();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact(DisplayName = "GetStoreById - Should return store when found")]
    public async Task StoreRepository_GetStoreById_ShouldReturnStoreWhenFound()
    {
        // Arrange
        using var context = new ApplicationDbContext(_dbContextOptions);
        var store = RepositoryTestFactory.CreateStore();

        context.Stores.Add(store);
        await context.SaveChangesAsync();

        var repository = new StoreRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetStoreById(store.Id);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(store.Id);
    }

    [Fact(DisplayName = "GetStoreById - Should return null when store not found")]
    public async Task StoreRepository_GetStoreById_ShouldReturnNullWhenNotFound()
    {
        // Arrange
        using var context = new ApplicationDbContext(_dbContextOptions);
        var repository = new StoreRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetStoreById(Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }

    [Fact(DisplayName = "GetStoreByName - Should return store when found")]
    public async Task StoreRepository_GetStoreByName_ShouldReturnStoreWhenFound()
    {
        // Arrange
        using var context = new ApplicationDbContext(_dbContextOptions);
        var store = RepositoryTestFactory.CreateStore();

        context.Stores.Add(store);
        await context.SaveChangesAsync();

        var repository = new StoreRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetStoreByName(store.Name);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(store.Name);
    }

    [Fact(DisplayName = "GetStoreByName - Should return null when store name not found")]
    public async Task StoreRepository_GetStoreByName_ShouldReturnNullWhenNotFound()
    {
        // Arrange
        using var context = new ApplicationDbContext(_dbContextOptions);
        var repository = new StoreRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetStoreByName("NonExistentStore");

        // Assert
        result.Should().BeNull();
    }

    [Fact(DisplayName = "AddStore - Should add store successfully")]
    public async Task StoreRepository_AddStore_ShouldAddStoreSuccessfully()
    {
        // Arrange
        using var context = new ApplicationDbContext(_dbContextOptions);
        var store = RepositoryTestFactory.CreateStore();
        var repository = new StoreRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.AddStore(store);

        // Assert
        result.Should().NotBeNull();
        var dbStore = await context.Stores.FindAsync(store.Id);
        dbStore.Should().NotBeNull();
        dbStore.Name.Should().Be(store.Name);
    }

    [Fact(DisplayName = "AddStore - Should throw DbUpdateException on failure")]
    public async Task StoreRepository_AddStore_ShouldThrowExceptionOnFailure()
    {
        // Arrange
        var failingContext = new FailingSaveChangesDbContext(_dbContextOptions);
        var repository = new StoreRepository(failingContext, _mockLogger.Object);
        var store = RepositoryTestFactory.CreateStore();

        // Act & Assert
        await Assert.ThrowsAsync<DbUpdateException>(() => repository.AddStore(store));
    }

    [Fact(DisplayName = "UpdateStore - Should update store successfully")]
    public async Task StoreRepository_UpdateStore_ShouldUpdateStoreSuccessfully()
    {
        // Arrange
        using var context = new ApplicationDbContext(_dbContextOptions);
        var store = RepositoryTestFactory.CreateStore();

        context.Stores.Add(store);
        await context.SaveChangesAsync();

        store.UpdateDetails("Updated Store Name", store.OwnerName);

        var repository = new StoreRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.UpdateStore(store);

        // Assert
        result.Name.Should().Be("Updated Store Name");
        var dbStore = await context.Stores.FindAsync(store.Id);
        dbStore.Name.Should().Be("Updated Store Name");
    }

    [Fact(DisplayName = "UpdateStore - Should throw DbUpdateException on failure")]
    public async Task StoreRepository_UpdateStore_ShouldThrowExceptionOnFailure()
    {
        // Arrange
        var failingContext = new FailingSaveChangesDbContext(_dbContextOptions);
        var repository = new StoreRepository(failingContext, _mockLogger.Object);
        var store = RepositoryTestFactory.CreateStore();

        // Act & Assert
        await Assert.ThrowsAsync<DbUpdateException>(() => repository.UpdateStore(store));
    }

    [Fact(DisplayName = "DeleteStore - Should delete store successfully")]
    public async Task StoreRepository_DeleteStore_ShouldDeleteStoreSuccessfully()
    {
        // Arrange
        using var context = new ApplicationDbContext(_dbContextOptions);
        var store = RepositoryTestFactory.CreateStore();

        context.Stores.Add(store);
        await context.SaveChangesAsync();

        var repository = new StoreRepository(context, _mockLogger.Object);

        // Act
        await repository.DeleteStore(store.Id);

        // Assert
        var dbStore = await context.Stores.FindAsync(store.Id);
        dbStore.Should().BeNull();
    }

    [Fact(DisplayName = "DeleteStore - Should throw DbUpdateException on failure")]
    public async Task StoreRepository_DeleteStore_ShouldThrowExceptionOnFailure()
    {
        // Arrange
        var failingContext = new FailingSaveChangesDbContext(_dbContextOptions);
        var repository = new StoreRepository(failingContext, _mockLogger.Object);
        var store = RepositoryTestFactory.CreateStore();

        using var context = new ApplicationDbContext(_dbContextOptions);
        context.Stores.Add(store);
        await context.SaveChangesAsync();

        // Act & Assert
        await Assert.ThrowsAsync<DbUpdateException>(() => repository.DeleteStore(store.Id));
    }
}