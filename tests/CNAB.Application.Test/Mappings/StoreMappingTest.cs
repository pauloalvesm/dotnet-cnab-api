using CNAB.Application.DTOs;
using CNAB.Application.Mappings;
using CNAB.Domain.Entities;
using CNAB.TestHelpers.Factories;
using FluentAssertions;
using Mapster;

namespace CNAB.Application.Test.Mappings;

public class StoreMappingTest
{
    private readonly TypeAdapterConfig _config;

    public StoreMappingTest()
    {
        _config = new TypeAdapterConfig();
        new StoreMapping().Register(_config);
    }

    [Fact(DisplayName = "StoreToStoreDto - Should map balance correctly")]
    public void StoreMapping_StoreToStoreDto_ShouldMapBalanceCorrectly()
    {
        // Arrange
        var store = ServiceTestFactory.CreateStore();
        store.AddTransaction(ServiceTestFactory.CreateTransaction());
        store.AddTransaction(ServiceTestFactory.CreateTransaction());

        // Act
        var storeDto = store.Adapt<StoreDto>(_config);

        // Assert
        storeDto.Should().NotBeNull();
        storeDto.Id.Should().Be(store.Id);
        storeDto.Name.Should().Be(store.Name);
        storeDto.OwnerName.Should().Be(store.OwnerName);
        storeDto.Balance.Should().Be(store.GetBalance());
    }

    [Fact(DisplayName = "StoreDtoToStore - Should ignore Transactions")]
    public void StoreMapping_StoreDtoToStore_ShouldIgnoreTransactions()
    {
        // Arrange
        var store = ServiceTestFactory.CreateStore();
        var storeDto = store.Adapt<StoreDto>(_config);

        // Act
        var storeMapped = storeDto.Adapt<Store>(_config);

        // Assert
        storeMapped.Should().NotBeNull();
        storeMapped.Id.Should().Be(storeDto.Id);
        storeMapped.Name.Should().Be(storeDto.Name);
        storeMapped.OwnerName.Should().Be(storeDto.OwnerName);
        storeMapped.Transactions.Should().BeEmpty();
    }

    [Fact(DisplayName = "StoreInputDtoToStore - Should ignore Id and Transactions")]
    public void StoreMapping_StoreInputDtoToStore_ShouldIgnoreIdAndTransactions()
    {
        // Arrange
        var storeInputDto = ServiceTestFactory.CreateStoreInputDto();

        // Act
        var store = storeInputDto.Adapt<Store>(_config);

        // Assert
        store.Should().NotBeNull();
        store.Id.Should().NotBe(storeInputDto.Id);
        store.Name.Should().Be(storeInputDto.Name);
        store.OwnerName.Should().Be(storeInputDto.OwnerName);
        store.Transactions.Should().BeEmpty();
    }

    [Fact(DisplayName = "NullStore - Should return null")]
    public void StoreMapping_NullStore_ShouldReturnNull()
    {
        // Arrange
        Store nullStore = null;

        // Act
        var storeDto = nullStore.Adapt<StoreDto>(_config);

        // Assert
        storeDto.Should().BeNull();
    }
}