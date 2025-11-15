using CNAB.Domain.Entities;
using CNAB.Domain.Entities.enums;
using CNAB.TestHelpers.Factories;
using FluentAssertions;

namespace CNAB.Domain.Test.Entities;

public class StoreTest
{
    [Fact(DisplayName = "Constructor - Should create Store when valid data")]
    public void Store_Constructor_ShouldCreateStoreWhenValidData()
    {
        // Arrange
        var name = "Store 01";
        var owner = "John Smith";

        // Act
        var store = new Store(name, owner);

        // Assert
        store.Name.Should().Be(name);
        store.OwnerName.Should().Be(owner);
        store.Id.Should().NotBe(Guid.Empty);
    }

    [Fact(DisplayName = "Constructor - Should throw exception when name is empty")]
    public void Store_Constructor_ShouldThrowExceptionWhenNameIsEmpty()
    {
        // Arrange
        var invalidName = "";
        var owner = "John Smith";

        // Act
        var act = () => new Store(invalidName, owner);

        // Assert
        act.Should()
            .Throw<Exception>()
            .WithMessage("*Invalid name*");
    }

    [Fact(DisplayName = "AddTransaction - Should add Transaction to list")]
    public void Store_AddTransaction_ShouldAddTransactionToList()
    {
        // Arrange
        var store = new Store("Store 02", "Mary");
        var transaction = EntityTestFactory.CreateValidTransaction(store);

        // Act
        store.AddTransaction(transaction);

        // Assert
        store.Transactions.Should().Contain(transaction);
    }

    [Fact(DisplayName = "AddTransaction - Should not throw when Transaction is valid")]
    public void Store_AddTransaction_ShouldNotThrowWhenTransactionIsValid()
    {
        // Arrange
        var store = new Store("Test Store", "Test Owner");
        var transaction = EntityTestFactory.CreateValidTransaction(store);

        // Act
        Action act = () => store.AddTransaction(transaction);

        // Assert
        act.Should().NotThrow();
    }

    [Theory(DisplayName = "GetBalance - Should return correct balance for given transactions")]
    [InlineData(TransactionType.Debit, 100, TransactionType.Bill, 50, 50)]
    [InlineData(TransactionType.Credit, 200, TransactionType.Financing, 100, 100)]
    [InlineData(TransactionType.Rent, 300, TransactionType.Rent, 200, -500)]
    [InlineData(TransactionType.Sales, 150, TransactionType.DOCReceipt, 150, 300)]
    [InlineData(TransactionType.Bill, 70, TransactionType.Bill, 30, -100)]
    public void Store_GetBalance_ShouldReturnCorrectSignedBalance(
        TransactionType type1, decimal amount1,
        TransactionType type2, decimal amount2,
        decimal expectedBalance)
    {
        // Arrange
        var store = new Store("Test Store", "Test Owner");

        var _typet1 = EntityTestFactory.CreateTransactionWithAmount(store, amount1, type1);
        var _typet2 = EntityTestFactory.CreateTransactionWithAmount(store, amount2, type2);

        store.AddTransaction(_typet1);
        store.AddTransaction(_typet2);

        // Act
        var balance = store.GetBalance();

        // Assert
        balance.Should().Be(expectedBalance);
    }

    [Fact(DisplayName = "GetBalance - Should return zero when no Transactions")]
    public void Store_GetBalance_ShouldReturnZeroWhenNoTransactions()
    {
        // Arrange
        var store = new Store("Test Store", "Test Owner");

        // Act
        var balance = store.GetBalance();

        // Assert
        balance.Should().Be(0);
    }

    [Fact(DisplayName = "UpdateDetails - Should update Store name and owner")]
    public void Store_UpdateDetails_ShouldUpdateStoreNameAndOwner()
    {
        // Arrange
        var store = new Store("Old Store", "Old Owner");

        // Act
        store.UpdateDetails("New Store", "New Owner");

        // Assert
        store.Name.Should().Be("New Store");
        store.OwnerName.Should().Be("New Owner");
    }

    [Fact(DisplayName = "UpdateDetails - Should throw exception when name in valid")]
    public void Store_UpdateDetails_ShouldThrowExceptionWhenNameInvalid()
    {
        // Arrange
        var store = new Store("Initial Store", "Initial Owner");

        // Act
        var act = () => store.UpdateDetails("", "New Owner");

        // Assert
        act.Should()
            .Throw<Exception>()
            .WithMessage("*Invalid name*");
    }
}