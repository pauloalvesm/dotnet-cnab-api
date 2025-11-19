using CNAB.Domain.Entities;
using CNAB.Domain.Entities.enums;
using CNAB.Domain.Validations;
using CNAB.Infra.Data.Test.Common;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace CNAB.Infra.Data.Test.Integrations;

public class TransactionIntegrationTest : IntegrationTestBase
{
    [Fact(DisplayName = "ApplicationDbContext - Can insert Transaction")]
    public void ApplicationDbContext_CanInsertTransaction()
    {
        // Arrange
        var store = new Store("Test Store", "Test Owner");
        DbContext.Stores.Add(store);
        DbContext.SaveChanges();

        var transaction = new Transaction(
            type: TransactionType.Debit,
            occurrenceDate: new DateTime(2019, 3, 1, 15, 34, 53),
            amount: 142.00m,
            cpf: "00962067601",
            cardNumber: "74753****3153",
            time: new TimeSpan(15, 34, 53),
            store: store
        );

        // Act
        DbContext.Transactions.Add(transaction);
        DbContext.SaveChanges();

        // Assert
        var insertedTransaction = DbContext.Transactions
            .Include(t => t.Store)
            .FirstOrDefault(t => t.CPF == "00962067601");

        insertedTransaction.Should().NotBeNull();
        insertedTransaction.Type.Should().Be(TransactionType.Debit);
        insertedTransaction.Amount.Should().Be(142.00m);
        insertedTransaction.CPF.Should().Be("00962067601");
        insertedTransaction.CardNumber.Should().Be("74753****3153");
        insertedTransaction.Store.Id.Should().Be(store.Id);
        insertedTransaction.Id.Should().NotBeEmpty();
    }

    [Fact(DisplayName = "ApplicationDbContext - Can query Transaction with Store relationship")]
    public void ApplicationDbContext_CanQueryTransactionWithStoreRelationship()
    {
        // Arrange
        var store = new Store("Relationship Store", "Relationship Owner");
        DbContext.Stores.Add(store);
        DbContext.SaveChanges();

        var transaction = new Transaction(
            type: TransactionType.Credit,
            occurrenceDate: new DateTime(2024, 1, 15),
            amount: 250.75m,
            cpf: "12345678901",
            cardNumber: "1111****2222",
            time: new TimeSpan(14, 20, 30),
            store: store
        );

        DbContext.Transactions.Add(transaction);
        DbContext.SaveChanges();

        // Act
        var retrievedTransaction = DbContext.Transactions
            .Include(t => t.Store)
            .FirstOrDefault(t => t.Id == transaction.Id);

        // Assert
        retrievedTransaction.Should().NotBeNull();
        retrievedTransaction.Store.Should().NotBeNull();
        retrievedTransaction.Store.Name.Should().Be("Relationship Store");
        retrievedTransaction.Store.OwnerName.Should().Be("Relationship Owner");
    }

    [Fact(DisplayName = "ApplicationDbContext - Can insert multiple Transactions for same Store")]
    public void ApplicationDbContext_CanInsertMultipleTransactionsForSameStore()
    {
        // Arrange
        var store = new Store("Multiple Transactions Store", "Multiple Owner");
        DbContext.Stores.Add(store);
        DbContext.SaveChanges();

        var transaction1 = new Transaction(
            TransactionType.Debit, DateTime.Now, 100.00m, "11111111111", "1111****1111", TimeSpan.FromHours(10), store);

        var transaction2 = new Transaction(
            TransactionType.Credit, DateTime.Now, 200.00m, "22222222222", "2222****2222", TimeSpan.FromHours(11), store);

        // Act
        DbContext.Transactions.AddRange(transaction1, transaction2);
        DbContext.SaveChanges();

        // Assert
        var storeTransactions = DbContext.Transactions
            .Where(t => t.Store.Id == store.Id)
            .ToList();

        storeTransactions.Should().HaveCount(2);
        storeTransactions.Should().Contain(t => t.CPF == "11111111111");
        storeTransactions.Should().Contain(t => t.CPF == "22222222222");
    }

    [Fact(DisplayName = "ApplicationDbContext - Cannot insert Transaction with invalid CPF due to Domain Validation")]
    public void ApplicationDbContext_CannotInsertTransaction_WithInvalidCpfDueToDomainValidation()
    {
        // Arrange
        var store = new Store("Valid Test Store", "Valid Test Owner");
        DbContext.Stores.Add(store);
        DbContext.SaveChanges();

        string invalidCpf = "123";

        // Act
        Action act = () =>
        {
            var transaction = new Transaction(
                type: TransactionType.Debit,
                occurrenceDate: new DateTime(2024, 5, 24),
                amount: 50.00m,
                cpf: invalidCpf,
                cardNumber: "1234567890123456",
                time: new TimeSpan(10, 0, 0),
                store: store
            );
            DbContext.Transactions.Add(transaction);
            DbContext.SaveChanges();
        };

        // Assert
        act.Should().Throw<DomainExceptionValidation>()
           .WithMessage("Invalid CPF, must be 11 characters");

        DbContext.Transactions.Should().BeEmpty();
    }

    [Fact(DisplayName = "ApplicationDbContext - Cannot insert Transaction with null CardNumber due to Domain Validation")]
    public void ApplicationDbContext_CannotInsertTransaction_WithNullCardNumberDueToDomainValidation()
    {
        // Arrange
        var store = new Store("Valid Test Store", "Valid Test Owner");
        DbContext.Stores.Add(store);
        DbContext.SaveChanges();

        string invalidCardNumber = null;

        // Act
        Action act = () =>
        {
            var transaction = new Transaction(
                type: TransactionType.Debit,
                occurrenceDate: new DateTime(2024, 5, 24),
                amount: 75.00m,
                cpf: "12345678901",
                cardNumber: invalidCardNumber,
                time: new TimeSpan(11, 30, 0),
                store: store
            );
            DbContext.Transactions.Add(transaction);
            DbContext.SaveChanges();
        };

        // Assert
        act.Should().Throw<DomainExceptionValidation>()
           .WithMessage("Invalid card number, Card number is required");

        DbContext.Transactions.Should().BeEmpty();
    }
}