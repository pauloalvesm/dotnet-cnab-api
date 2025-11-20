using CNAB.Application.DTOs;
using CNAB.Application.Mappings;
using CNAB.Domain.Entities;
using CNAB.Domain.Entities.enums;
using CNAB.TestHelpers.Factories;
using FluentAssertions;
using Mapster;

namespace CNAB.Application.Test.Mappings;

public class TransactionMappingTest
{
    private readonly TypeAdapterConfig _config;

    public TransactionMappingTest()
    {
        _config = new TypeAdapterConfig();
        new TransactionMapping().Register(_config);
    }

    [Fact(DisplayName = "TransactionToTransactionDto - Should map all properties correctly")]
    public void TransactionMapping_TransactionToTransactionDto_ShouldMapAllPropertiesCorrectly()
    {
        // Arrange
        var store = ServiceTestFactory.CreateStore();
        var transaction = ServiceTestFactory.CreateTransaction(store);

        // Act
        var transactionDto = transaction.Adapt<TransactionDto>(_config);

        // Assert
        transactionDto.Should().NotBeNull();
        transactionDto.Id.Should().Be(transaction.Id);
        transactionDto.Type.Should().Be((int)transaction.Type);
        transactionDto.OccurrenceDate.Should().Be(transaction.OccurrenceDate);
        transactionDto.Amount.Should().Be(transaction.Amount);
        transactionDto.CPF.Should().Be(transaction.CPF);
        transactionDto.CardNumber.Should().Be(transaction.CardNumber);
        transactionDto.Time.Should().Be(transaction.Time);
        transactionDto.StoreId.Should().Be(transaction.Store.Id);
        transactionDto.StoreName.Should().Be(transaction.Store.Name);
        transactionDto.StoreOwnerName.Should().Be(transaction.Store.OwnerName);
    }

    [Fact(DisplayName = "TransactionDtoToTransaction - Should ignore store and derived properties")]
    public void TransactionMapping_TransactionDtoToTransaction_ShouldIgnoreStoreAndDerivedProperties()
    {
        // Arrange
        var store = ServiceTestFactory.CreateStore();
        var transaction = ServiceTestFactory.CreateTransaction(store);
        var transactionDto = transaction.Adapt<TransactionDto>(_config);

        // Act
        var mappedTransaction = transactionDto.Adapt<Transaction>(_config);

        // Assert
        mappedTransaction.Should().NotBeNull();
        mappedTransaction.Id.Should().Be(transactionDto.Id);
        mappedTransaction.Type.Should().Be((TransactionType)transactionDto.Type);
        mappedTransaction.OccurrenceDate.Should().Be(transactionDto.OccurrenceDate);
        mappedTransaction.Amount.Should().Be(transactionDto.Amount);
        mappedTransaction.CPF.Should().Be(transactionDto.CPF);
        mappedTransaction.CardNumber.Should().Be(transactionDto.CardNumber);
        mappedTransaction.Time.Should().Be(transactionDto.Time);

        mappedTransaction.Store.Should().BeNull();

        mappedTransaction.SignedAmount.Should().Be(mappedTransaction.IsIncome ? mappedTransaction.Amount : -mappedTransaction.Amount);
    }

    [Fact(DisplayName = "TransactionDtoToTransaction - With invalid type should map with invalid enum value")]
    public void TransactionMapping_TransactionDtoToTransaction_WithInvalidTypeShouldMapWithInvalidEnumValue()
    {
        // Arrange
        var transactionDto = ServiceTestFactory.CreateTransactionDtoInvalidEnum();

        // Act
        var mappedTransaction = transactionDto.Adapt<Transaction>(_config);

        // Assert
        ((int)mappedTransaction.Type).Should().Be(999);

        bool isValidEnum = Enum.IsDefined(typeof(TransactionType), mappedTransaction.Type);
        isValidEnum.Should().BeFalse();
    }
}