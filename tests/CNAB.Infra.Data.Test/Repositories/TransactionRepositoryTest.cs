using CNAB.Domain.Entities;
using CNAB.Domain.Interfaces.Repositories;
using CNAB.Infra.Data.Context;
using CNAB.Infra.Data.Repositories;
using CNAB.Infra.Data.Test.Common;
using CNAB.TestHelpers.Factories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace CNAB.Infra.Data.Test.Repositories;

public class TransactionRepositoryTest
{
    private readonly Mock<ApplicationDbContext> _mockContext;
    private readonly Mock<ILogger<TransactionRepository>> _mockLogger;
    private readonly ITransactionRepository _transactionRepository;
    private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;

    public TransactionRepositoryTest()
    {
        _mockLogger = new Mock<ILogger<TransactionRepository>>();

        _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new ApplicationDbContext(_dbContextOptions);
        _mockContext = new Mock<ApplicationDbContext>(_dbContextOptions) { CallBase = true };

        _transactionRepository = new TransactionRepository(context, _mockLogger.Object);
    }

    [Fact(DisplayName = "GetAllTransactions - Should return all transactions")]
    public async Task TransactionRepository_GetAllTransactions_ShouldReturnAllTransactions()
    {
        // Arrange
        using var context = new ApplicationDbContext(_dbContextOptions);
        var transactions = RepositoryTestFactory.GenerateListTransactions();

        context.Transactions.AddRange(transactions);
        await context.SaveChangesAsync();

        var repository = new TransactionRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetAllTransactions();

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().HaveCount(transactions.Count);
    }

    [Fact(DisplayName = "GetAllTransactions - Should return empty list when no Transactions")]
    public async Task TransactionRepository_GetAllTransactions_ShouldReturnEmptyListWhenNoTransactions()
    {
        // Arrange
        using var context = new ApplicationDbContext(_dbContextOptions);
        var repository = new TransactionRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetAllTransactions();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact(DisplayName = "GetTransactionById - Should return Transaction when found")]
    public async Task TransactionRepository_GetTransactionById_ShouldReturnTransactionWhenFound()
    {
        // Arrange
        using var context = new ApplicationDbContext(_dbContextOptions);
        var transaction = RepositoryTestFactory.CreateTransaction();
        context.Transactions.Add(transaction);
        await context.SaveChangesAsync();

        var repository = new TransactionRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetTransactionById(transaction.Id);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(transaction.Id);
    }

    [Fact(DisplayName = "GetTransactionById - Should return Null when not found")]
    public async Task TransactionRepository_GetTransactionById_ShouldReturnNullWhenNotFound()
    {
        // Arrange
        using var context = new ApplicationDbContext(_dbContextOptions);
        var repository = new TransactionRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetTransactionById(Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }

    [Fact(DisplayName = "AddTransaction - Should add Transaction successfully")]
    public async Task TransactionRepository_AddTransaction_ShouldAddTransactionSuccessfully()
    {
        // Arrange
        using var context = new ApplicationDbContext(_dbContextOptions);
        var transaction = RepositoryTestFactory.CreateTransaction();
        var repository = new TransactionRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.AddTransaction(transaction);

        // Assert
        result.Should().NotBeNull();
        var dbTransaction = await context.Transactions.FindAsync(transaction.Id);
        dbTransaction.Should().NotBeNull();
        dbTransaction.Amount.Should().Be(transaction.Amount);
    }

    [Fact(DisplayName = "AddTransaction - Should throw NullReferenceException when Transaction is null")]
    public async Task TransactionRepository_AddTransaction_ShouldThrowNullReferenceException_WhenTransactionIsNull()
    {
        // Arrange
        using var context = new ApplicationDbContext(_dbContextOptions);
        var repository = new TransactionRepository(context, _mockLogger.Object);
        Transaction invalidTransaction = null!;

        // Act & Assert
        await Assert.ThrowsAsync<NullReferenceException>(() => repository.AddTransaction(invalidTransaction));
    }


    [Fact(DisplayName = "UpdateTransaction - Should update Transaction successfully")]
    public async Task TransactionRepository_UpdateTransaction_ShouldUpdateTransactionSuccessfully()
    {
        // Arrange
        using var context = new ApplicationDbContext(_dbContextOptions);
        var transaction = RepositoryTestFactory.CreateTransaction();
        context.Transactions.Add(transaction);
        await context.SaveChangesAsync();

        var repository = new TransactionRepository(context, _mockLogger.Object);

        transaction.UpdateDetails(
            transaction.Type,
            transaction.OccurrenceDate,
            500.00m, 
            transaction.CPF,
            transaction.CardNumber,
            transaction.Time
        );

        // Act
        var result = await repository.UpdateTransaction(transaction);

        // Assert
        result.Amount.Should().Be(500.00m);
        var dbTransaction = await context.Transactions.FindAsync(transaction.Id);
        dbTransaction.Amount.Should().Be(500.00m);
    }


    [Fact(DisplayName = "UpdateTransaction - Should throw exception when update fails")]
    public async Task TransactionRepository_UpdateTransaction_ShouldThrowExceptionWhenUpdateFails()
    {
        // Arrange
        var repository = new TransactionRepository(_mockContext.Object, _mockLogger.Object);
        var transaction = RepositoryTestFactory.CreateTransaction();

        _mockContext.Setup(m => m.SaveChangesAsync(default))
            .ThrowsAsync(new DbUpdateException("Simulated exception"));

        // Act & Assert
        await Assert.ThrowsAsync<DbUpdateException>(() => repository.UpdateTransaction(transaction));
    }

    [Fact(DisplayName = "DeleteTransaction - Should delete Transaction successfully")]
    public async Task TransactionRepository_DeleteTransaction_ShouldDeleteTransactionSuccessfully()
    {
        // Arrange
        using var context = new ApplicationDbContext(_dbContextOptions);
        var transaction = RepositoryTestFactory.CreateTransaction();
        context.Transactions.Add(transaction);
        await context.SaveChangesAsync();

        var repository = new TransactionRepository(context, _mockLogger.Object);

        // Act
        await repository.DeleteTransaction(transaction.Id);

        // Assert
        var dbTransaction = await context.Transactions.FindAsync(transaction.Id);
        dbTransaction.Should().BeNull();
    }

    [Fact(DisplayName = "DeleteTransaction - Should throw exception when deletion fails")]
    public async Task TransactionRepository_DeleteTransaction_ShouldThrowExceptionWhenDeletionFails()
    {
        // Arrange
        using var context = new ApplicationDbContext(_dbContextOptions);
        var transaction = RepositoryTestFactory.CreateTransaction();
        context.Transactions.Add(transaction);
        await context.SaveChangesAsync();

        var failingContext = new FailingSaveChangesDbContext(_dbContextOptions);
        var repository = new TransactionRepository(failingContext, _mockLogger.Object);

        // Act & Assert
        await Assert.ThrowsAsync<DbUpdateException>(() => repository.DeleteTransaction(transaction.Id));
    }
}