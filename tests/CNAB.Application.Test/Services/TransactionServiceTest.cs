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

public class TransactionServiceTest
{
    private readonly Mock<ITransactionRepository> _transactionRepositoryMock;
    private readonly Mock<IStoreRepository> _storeRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<TransactionService>> _loggerMock;
    private readonly TransactionService _transactionService;

    public TransactionServiceTest()
    {
        _transactionRepositoryMock = new Mock<ITransactionRepository>();
        _storeRepositoryMock = new Mock<IStoreRepository>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<TransactionService>>();
        _transactionService = new TransactionService(
            _transactionRepositoryMock.Object,
            _storeRepositoryMock.Object,
            _mapperMock.Object,
            _loggerMock.Object);
    }

    [Fact(DisplayName = "GetAllTransactionsAsyn - Should return all Transactions")]
    public async Task TransactionService_GetAllTransactionsAsync_ShouldReturnAllTransactions()
    {
        // Arrange
        var transactions = new List<Transaction>
        {
            ServiceTestFactory.CreateTransaction(Guid.NewGuid()),
            ServiceTestFactory.CreateTransaction(Guid.NewGuid())
        };

        var transactionDtos = new List<TransactionDto>
        {
            ServiceTestFactory.CreateTransactionDto(transactions[0].Id),
            ServiceTestFactory.CreateTransactionDto(transactions[1].Id)
        };

        _transactionRepositoryMock.Setup(repo => repo.GetAllTransactions())
            .ReturnsAsync(transactions);

        _mapperMock.Setup(mapper => mapper.Map<IEnumerable<TransactionDto>>(transactions))
            .Returns(transactionDtos);

        // Act
        var result = await _transactionService.GetAllTransactionsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _transactionRepositoryMock.Verify(repo => repo.GetAllTransactions(), Times.Once);
        _mapperMock.Verify(mapper => mapper.Map<IEnumerable<TransactionDto>>(transactions), Times.Once);
    }

    [Fact(DisplayName = "GetAllStoreAsync - Should Return empty list when no Transactions exist")]
    public async Task TransactionService_GetAllTransactionsAsync_ShouldReturnEmptyListWhenNoTransactionsExist()
    {
        // Arrange
        var emptyTransactionList = new List<Transaction>();
        var emptyDtoList = new List<TransactionDto>();

        _transactionRepositoryMock.Setup(repo => repo.GetAllTransactions())
            .ReturnsAsync(emptyTransactionList);

        _mapperMock.Setup(mapper => mapper.Map<IEnumerable<TransactionDto>>(emptyTransactionList))
            .Returns(emptyDtoList);

        // Act
        var result = await _transactionService.GetAllTransactionsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
        _transactionRepositoryMock.Verify(repo => repo.GetAllTransactions(), Times.Once);
    }

    [Fact(DisplayName = "GetTransactionByIdAsync - Should return TransactionDto when Id exists")]
    public async Task TransactionService_GetTransactionByIdAsync_ShouldReturnTransactionDtoWhenIdExists()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var transaction = ServiceTestFactory.CreateTransaction(transactionId);
        var transactionDto = ServiceTestFactory.CreateTransactionDto(transactionId);

        _transactionRepositoryMock.Setup(repo => repo.GetTransactionById(transactionId))
            .ReturnsAsync(transaction);

        _mapperMock.Setup(mapper => mapper.Map<TransactionDto>(transaction))
            .Returns(transactionDto);

        // Act
        var result = await _transactionService.GetTransactionByIdAsync(transactionId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(transactionId, result.Id);
        _transactionRepositoryMock.Verify(repo => repo.GetTransactionById(transactionId), Times.Once);
        _mapperMock.Verify(mapper => mapper.Map<TransactionDto>(transaction), Times.Once);
    }

    [Fact(DisplayName = "GetTransactionByIdAsync - Should return null when TransactionDto Id does not exist")]
    public async Task TransactionService_GetTransactionByIdAsync_ShouldReturnNullWhenTransactionDtoIdDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        _transactionRepositoryMock.Setup(repo => repo.GetTransactionById(nonExistentId))
            .ReturnsAsync((Transaction)null);

        // Act
        var result = await _transactionService.GetTransactionByIdAsync(nonExistentId);

        // Assert
        Assert.Null(result);
        _transactionRepositoryMock.Verify(repo => repo.GetTransactionById(nonExistentId), Times.Once);
        _loggerMock.Verify(
            logger => logger.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Transaction not found")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Fact(DisplayName = "AddTransactionAsync - Should insert TransactionDto and return TransactionDto")]
    public async Task TransactionService_AddTransactionAsync_ShouldInsertTransactionDtoAndReturnTransactionDto()
    {
        // Arrange
        var storeId = Guid.NewGuid();
        var transactionDto = ServiceTestFactory.CreateTransactionDto(Guid.NewGuid());
        transactionDto.StoreId = storeId;

        var store = new Store("Test Store", "Test Owner");
        var transaction = new Transaction(
            (TransactionType)transactionDto.Type,
            transactionDto.OccurrenceDate,
            transactionDto.Amount,
            transactionDto.CPF,
            transactionDto.CardNumber,
            transactionDto.Time,
            store);

        _storeRepositoryMock.Setup(repo => repo.GetStoreById(storeId))
            .ReturnsAsync(store);

        _transactionRepositoryMock.Setup(repo => repo.AddTransaction(It.IsAny<Transaction>()))
            .ReturnsAsync(transaction);

        _mapperMock.Setup(mapper => mapper.Map<TransactionDto>(It.IsAny<Transaction>()))
            .Returns(transactionDto);

        // Act
        var result = await _transactionService.AddTransactionAsync(transactionDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(transactionDto.Id, result.Id);
        _storeRepositoryMock.Verify(repo => repo.GetStoreById(storeId), Times.Once);
        _transactionRepositoryMock.Verify(repo => repo.AddTransaction(It.IsAny<Transaction>()), Times.Once);
        _mapperMock.Verify(mapper => mapper.Map<TransactionDto>(It.IsAny<Transaction>()), Times.Once);
    }

    [Fact(DisplayName = "AddTransactionAsync - Should return null when StoreId does not exist")]
    public async Task TransactionService_AddTransactionAsync_ShouldReturnNullWhenStoreIdDoesNotExist()
    {
        // Arrange
        var nonExistentStoreId = Guid.NewGuid();
        var transactionDto = ServiceTestFactory.CreateTransactionDto(Guid.NewGuid());
        transactionDto.StoreId = nonExistentStoreId;

        _storeRepositoryMock.Setup(repo => repo.GetStoreById(nonExistentStoreId))
            .ReturnsAsync((Store)null);

        // Act
        var result = await _transactionService.AddTransactionAsync(transactionDto);

        // Assert
        Assert.Null(result);
        _storeRepositoryMock.Verify(repo => repo.GetStoreById(nonExistentStoreId), Times.Once);
        _transactionRepositoryMock.Verify(repo => repo.AddTransaction(It.IsAny<Transaction>()), Times.Never);
        _loggerMock.Verify(
            logger => logger.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Store not found")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Fact(DisplayName = "UpdateTransactionAsync - Should update and return updated TransactionDto")]
    public async Task TransactionService_UpdateTransactionAsync_ShouldUpdateAndReturnUpdatedTransactionDto()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var storeId = Guid.NewGuid();
        var store = new Store("Test Store", "Test Owner");
        var existingTransaction = ServiceTestFactory.UpdateTransaction(storeId, store);
        var transactionDto = ServiceTestFactory.UpdateTransactionDto(transactionId, storeId);

        _transactionRepositoryMock.Setup(repo => repo.GetTransactionById(transactionId))
            .ReturnsAsync(existingTransaction);

        _transactionRepositoryMock.Setup(repo => repo.UpdateTransaction(existingTransaction))
            .ReturnsAsync(existingTransaction);

        _mapperMock.Setup(mapper => mapper.Map<TransactionDto>(existingTransaction))
            .Returns(transactionDto);

        // Act
        var result = await _transactionService.UpdateTransactionAsync(transactionDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(transactionId, result.Id);
        _transactionRepositoryMock.Verify(repo => repo.GetTransactionById(transactionId), Times.Once);
        _transactionRepositoryMock.Verify(repo => repo.UpdateTransaction(existingTransaction), Times.Once);
        _mapperMock.Verify(mapper => mapper.Map<TransactionDto>(existingTransaction), Times.Once);
    }

    [Fact(DisplayName = "UpdateTransactionAsync - Should return null when TransactionDto not found")]
    public async Task TransactionService_UpdateTransactionAsync_ShouldReturnNullWhenTransactionDtoNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var transactionDto = ServiceTestFactory.CreateTransactionDto(nonExistentId);

        _transactionRepositoryMock.Setup(repo => repo.GetTransactionById(nonExistentId))
            .ReturnsAsync((Transaction)null);

        // Act
        var result = await _transactionService.UpdateTransactionAsync(transactionDto);

        // Assert
        Assert.Null(result);
        _transactionRepositoryMock.Verify(repo => repo.GetTransactionById(nonExistentId), Times.Once);
        _transactionRepositoryMock.Verify(repo => repo.UpdateTransaction(It.IsAny<Transaction>()), Times.Never);
        _loggerMock.Verify(
            logger => logger.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Transaction not found")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Fact(DisplayName = "DeleteTransactionAsync - Should delete Transaction when exists")]
    public async Task TransactionService_DeleteTransactionAsync_ShouldDeleteTransactionWhenExists()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var transaction = ServiceTestFactory.CreateTransaction(transactionId);

        _transactionRepositoryMock.Setup(repo => repo.GetTransactionById(transactionId))
            .ReturnsAsync(transaction);

        _transactionRepositoryMock.Setup(repo => repo.DeleteTransaction(transactionId))
            .Returns(Task.CompletedTask);

        // Act
        await _transactionService.DeleteTransactionAsync(transactionId);

        // Assert
        _transactionRepositoryMock.Verify(repo => repo.GetTransactionById(transactionId), Times.Once);
        _transactionRepositoryMock.Verify(repo => repo.DeleteTransaction(transactionId), Times.Once);
    }

    [Fact(DisplayName = "DeleteTransactionAsync - Should LogError when Transaction not found")]
    public async Task TransactionService_DeleteTransactionAsync_ShouldLogErrorWhenTransactionNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        _transactionRepositoryMock.Setup(repo => repo.GetTransactionById(nonExistentId))
            .ReturnsAsync((Transaction)null);

        _transactionRepositoryMock.Setup(repo => repo.DeleteTransaction(nonExistentId))
            .Returns(Task.CompletedTask);

        // Act
        await _transactionService.DeleteTransactionAsync(nonExistentId);

        // Assert
        _transactionRepositoryMock.Verify(repo => repo.GetTransactionById(nonExistentId), Times.Once);
        _transactionRepositoryMock.Verify(repo => repo.DeleteTransaction(nonExistentId), Times.Once);
        _loggerMock.Verify(
            logger => logger.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Transaction not found")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }
}