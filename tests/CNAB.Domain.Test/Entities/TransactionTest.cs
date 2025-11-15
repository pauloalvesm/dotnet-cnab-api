using CNAB.Domain.Entities;
using CNAB.Domain.Entities.enums;
using CNAB.TestHelpers.Factories;
using FluentAssertions;

namespace CNAB.Domain.Test.Entities;

public class TransactionTest
{
    [Fact(DisplayName = "Constructor - Should create Transaction when data is valid")]
    public void Transaction_Constructor_ShouldCreateTransactionWhenDataIsValid()
    {
        // Arrange
        var store = new Store("Test Store", "John Doe");

        // Act
        var transaction = new Transaction(
            TransactionType.Credit,
            new DateTime(2024, 5, 10),
            100.50m,
            "12345678901",
            "1234****5678",
            new TimeSpan(14, 30, 0),
            store
        );

        // Assert
        transaction.Should().NotBeNull();
        transaction.Amount.Should().Be(100.50m);
        transaction.Type.Should().Be(TransactionType.Credit);
        transaction.CPF.Should().Be("12345678901");
        transaction.CardNumber.Should().Be("1234****5678");
        transaction.Store.Should().Be(store);
    }

    [Fact(DisplayName = "Constructor - Should throw exception when CPF is invalid")]
    public void Transaction_Constructor_ShouldThrowExceptionWhenCpfIsInvalid()
    {
        // Arrange
        var store = new Store("Test Store", "John Doe");

        // Act
        var act = () => new Transaction(
            TransactionType.Credit,
            DateTime.Now,
            100.50m,
            "123",
            "1234****5678",
            TimeSpan.FromHours(14),
            store
        );

        // Assert
        act.Should().Throw<Exception>()
            .WithMessage("*must be 11 characters*");
    }

    [Fact(DisplayName = "UpdateDetails - Should update Transaction when data is valid")]
    public void Transaction_UpdateDetails_ShouldUpdateTransactionWhenDataIsValid()
    {
        // Arrange
        var store = new Store("Test Store", "John Doe");
        var transaction = EntityTestFactory.CreateValidTransaction(store);

        // Act
        transaction.UpdateDetails(
            TransactionType.Sales,
            new DateTime(2024, 4, 5),
            250.75m,
            "98765432100",
            "9876****4321",
            new TimeSpan(9, 45, 0)
        );

        // Assert
        transaction.Type.Should().Be(TransactionType.Sales);
        transaction.Amount.Should().Be(250.75m);
        transaction.CPF.Should().Be("98765432100");
        transaction.CardNumber.Should().Be("9876****4321");
    }

    [Fact(DisplayName = "UpdateDetails - Should throw exception when amount is zero")]
    public void Transaction_UpdateDetails_ShouldThrowExceptionWhenAmountIsZero()
    {
        // Arrange
        var store = new Store("Test Store", "John Doe");
        var transaction = EntityTestFactory.CreateValidTransaction(store);

        // Act
        var act = () => transaction.UpdateDetails(
            TransactionType.Credit,
            DateTime.Now,
            0m,
            "12345678901",
            "1234****5678",
            TimeSpan.FromHours(10)
        );

        // Assert
        act.Should().Throw<Exception>()
            .WithMessage("*must be greater than zero*");
    }

    [Theory(DisplayName = "SignedAmount - Should return correct signed value")]
    [InlineData(TransactionType.Debit, 100, 100)]
    [InlineData(TransactionType.Credit, 200, 200)]
    [InlineData(TransactionType.Sales, 150, 150)]
    [InlineData(TransactionType.Bill, 120, -120)]
    [InlineData(TransactionType.Financing, 80, -80)]
    [InlineData(TransactionType.Rent, 50, -50)]
    public void Transaction_SignedAmount_ShouldReturnCorrectValue(TransactionType type, decimal amount, decimal expectedSigned)
    {
        // Arrange
        var store = EntityTestFactory.CreateStore();
        var transaction = EntityTestFactory.CreateTransactionWithAmount(store, amount, type);

        // Act
        var signedAmount = transaction.SignedAmount;

        // Assert
        signedAmount.Should().Be(expectedSigned);
    }

    [Theory(DisplayName = "IsIncome - Should return true for income transaction types")]
    [InlineData(TransactionType.Debit)]
    [InlineData(TransactionType.Credit)]
    [InlineData(TransactionType.LoanReceipt)]
    [InlineData(TransactionType.Sales)]
    [InlineData(TransactionType.TEDReceipt)]
    [InlineData(TransactionType.DOCReceipt)]
    public void Transaction_IsIncome_ShouldBeTrue(TransactionType type)
    {
        // Arrange
        var store = EntityTestFactory.CreateStore();
        var transaction = EntityTestFactory.CreateTransactionWithAmount(store, 100, type);

        // Act + Assert
        transaction.IsIncome.Should().BeTrue();
        transaction.IsExpense.Should().BeFalse();
    }

    [Theory(DisplayName = "IsExpense - Should return true for expense transaction types")]
    [InlineData(TransactionType.Bill)]
    [InlineData(TransactionType.Financing)]
    [InlineData(TransactionType.Rent)]
    public void Transaction_IsExpense_ShouldBeTrue(TransactionType type)
    {
        // Arrange
        var store = EntityTestFactory.CreateStore();
        var transaction = EntityTestFactory.CreateTransactionWithAmount(store, 100, type);

        // Act + Assert
        transaction.IsExpense.Should().BeTrue();
        transaction.IsIncome.Should().BeFalse();
    }
}