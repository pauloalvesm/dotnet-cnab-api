using CNAB.Application.Services;
using CNAB.Domain.Entities;
using CNAB.Domain.Entities.enums;
using CNAB.Domain.Interfaces.Repositories;
using CNAB.TestHelpers.Factories;
using MapsterMapper;
using Moq;

namespace CNAB.Application.Test.Services;

public class CNABProcessingServiceTest
{
    private readonly Mock<ITransactionRepository> _transactionRepoMock;
    private readonly Mock<IStoreRepository> _storeRepoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CNABProcessingService _service;
    private static readonly string ValidLinePadded = ServiceTestFactory.GetValidCnabLinePadded();
    private const string InvalidLine = "123";

    public CNABProcessingServiceTest()
    {
        _transactionRepoMock = new Mock<ITransactionRepository>();
        _storeRepoMock = new Mock<IStoreRepository>();
        _mapperMock = new Mock<IMapper>();

        _service = new CNABProcessingService(
            _transactionRepoMock.Object,
            _storeRepoMock.Object,
            _mapperMock.Object);
    }

    [Fact(DisplayName = "ParseCNABAsync - Should return failure when invalid lines exist")]
    public async Task CNABProcessingService_ParseCNABAsync_ShouldReturnFailureWhenInvalidLinesExist()
    {
        // Arrange
        var lines = new List<string> { ValidLinePadded, InvalidLine };

        // Act
        var result = await _service.ParseCNABAsync(lines);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Invalid line lengths", result.Message);
        Assert.Equal(0, result.TotalProcessed);
    }

    [Fact(DisplayName = "ParseCNABAsync - Should return success when all lines are valid")]
    public async Task CNABProcessingService_ParseCNABAsync_ShouldReturnSuccessWhenAllLinesAreValid()
    {
        // Arrange
        var lines = new List<string> { ValidLinePadded, ValidLinePadded };
        _transactionRepoMock.Setup(r => r.AddTransaction(It.IsAny<Transaction>())).ReturnsAsync((Transaction t) => t);
        _storeRepoMock.Setup(r => r.GetStoreByName(It.IsAny<string>())).ReturnsAsync((Store)null);
        _storeRepoMock.Setup(r => r.AddStore(It.IsAny<Store>())).ReturnsAsync((Store s) => s);

        // Act
        var result = await _service.ParseCNABAsync(lines);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(2, result.TotalProcessed);
        Assert.Equal("File processed successfully.", result.Message);
    }

    [Fact(DisplayName = "ParseCNABAsync - Should handle empty list")]
    public async Task CNABProcessingService_ParseCNABAsync_ShouldHandleEmptyList()
    {
        // Arrange
        var lines = new List<string>();

        // Act
        var result = await _service.ParseCNABAsync(lines);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(0, result.TotalProcessed);
    }

    [Fact(DisplayName = "ProcessValidLinesAsync - Should process all valid lines")]
    public async Task CNABProcessingService_ProcessValidLinesAsync_ShouldProcessAllValidLines()
    {
        // Arrange
        var lines = new List<string> { ValidLinePadded, ValidLinePadded };
        _transactionRepoMock.Setup(r => r.AddTransaction(It.IsAny<Transaction>())).ReturnsAsync((Transaction t) => t);
        _storeRepoMock.Setup(r => r.GetStoreByName(It.IsAny<string>())).ReturnsAsync((Store)null);
        _storeRepoMock.Setup(r => r.AddStore(It.IsAny<Store>())).ReturnsAsync((Store s) => s);

        // Act
        var processed = await _service.ProcessValidLinesAsync(lines);

        // Assert
        Assert.Equal(2, processed);
        _transactionRepoMock.Verify(r => r.AddTransaction(It.IsAny<Transaction>()), Times.Exactly(2));
    }

    [Fact(DisplayName = "ProcessValidLinesAsync - Should return zero when list is empty")]
    public async Task CNABProcessingService_ProcessValidLinesAsync_ShouldReturnZeroWhenListIsEmpty()
    {
        // Arrange
        var lines = new List<string>();

        // Act
        var processed = await _service.ProcessValidLinesAsync(lines);

        // Assert
        Assert.Equal(0, processed);
    }

    [Fact(DisplayName = "ProcessValidLinesAsync - Should handle exception from repository")]
    public async Task CNABProcessingService_ProcessValidLinesAsync_ShouldHandleRepositoryException()
    {
        // Arrange
        var lines = new List<string> { ValidLinePadded };
        _transactionRepoMock.Setup(r => r.AddTransaction(It.IsAny<Transaction>())).ThrowsAsync(new Exception("DB error"));
        _storeRepoMock.Setup(r => r.GetStoreByName(It.IsAny<string>())).ReturnsAsync((Store)null);
        _storeRepoMock.Setup(r => r.AddStore(It.IsAny<Store>())).ReturnsAsync((Store s) => s);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.ProcessValidLinesAsync(lines));
    }

    [Fact(DisplayName = "ParseLineAsync - Should parse valid line to Transaction")]
    public async Task CNABProcessingService_ParseLineAsync_ShouldParseValidLineToTransaction()
    {
        // Arrange
        _storeRepoMock.Setup(r => r.GetStoreByName(It.IsAny<string>())).ReturnsAsync((Store)null);
        _storeRepoMock.Setup(r => r.AddStore(It.IsAny<Store>())).ReturnsAsync((Store s) => s);

        // Act
        var transaction = await _service.ParseLineAsync(ValidLinePadded);

        // Assert
        Assert.Equal(TransactionType.Debit, transaction.Type);
        Assert.Equal(new DateTime(2019, 03, 01, 15, 34, 53), transaction.OccurrenceDate);
        Assert.Equal(142.00m, transaction.Amount);
        Assert.Equal("00962067601", transaction.CPF);
        Assert.Equal("74753****315", transaction.CardNumber);
        Assert.Equal("JOHN DOE", transaction.Store.OwnerName.Trim());
        Assert.Equal("JOHN'S Bar", transaction.Store.Name.Trim());
    }

    [Fact(DisplayName = "ParseLineAsync - Should throw ArgumentOutOfRangeException on invalid line")]
    public async Task CNABProcessingService_ParseLineAsync_ShouldThrowArgumentOutOfRangeExceptionOnInvalidLine()
    {
        // Arrange, Act & Assert
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _service.ParseLineAsync(InvalidLine));
    }

    [Fact(DisplayName = "GetOrCreateStoreAsync - Should return existing Store if found")]
    public async Task CNABProcessingService_GetOrCreateStoreAsync_ShouldReturnExistingStoreIfFound()
    {
        // Arrange
        var store = ServiceTestFactory.CreateStore();
        _storeRepoMock.Setup(r => r.GetStoreByName(store.Name)).ReturnsAsync(store);

        // Act
        var result = await _service.GetOrCreateStoreAsync(store.Name, store.OwnerName);

        // Assert
        Assert.Equal(store, result);
        _storeRepoMock.Verify(r => r.GetStoreByName(store.Name), Times.Once);
        _storeRepoMock.Verify(r => r.AddStore(It.IsAny<Store>()), Times.Never);
    }

    [Fact(DisplayName = "GetOrCreateStoreAsync - Should create Store when not found")]
    public async Task CNABProcessingService_GetOrCreateStoreAsync_ShouldCreateStoreWhenNotFound()
    {
        // Arrange
        var storeName = "New Store";
        var ownerName = "New Owner";
        _storeRepoMock.Setup(r => r.GetStoreByName(storeName)).ReturnsAsync((Store)null);
        _storeRepoMock.Setup(r => r.AddStore(It.IsAny<Store>())).ReturnsAsync((Store s) => s);

        // Act
        var result = await _service.GetOrCreateStoreAsync(storeName, ownerName);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(storeName, result.Name);
        Assert.Equal(ownerName, result.OwnerName);
        _storeRepoMock.Verify(r => r.AddStore(It.IsAny<Store>()), Times.Once);
    }

    [Fact(DisplayName = "SaveTransactionAsync - Should call AddAsync on repository")]
    public async Task CNABProcessingService_SaveTransactionAsync_ShouldCallAddAsyncOnRepository()
    {
        // Arrange
        var transaction = ServiceTestFactory.CreateTransaction();
        _transactionRepoMock.Setup(r => r.AddTransaction(transaction)).ReturnsAsync(transaction);

        // Act
        await _service.SaveTransactionAsync(transaction);

        // Assert
        _transactionRepoMock.Verify(r => r.AddTransaction(transaction), Times.Once);
    }

    [Fact(DisplayName = "SaveTransactionAsync - Should ignore null transaction without throwing")]
    public async Task CNABProcessingService_SaveTransactionAsync_ShouldIgnoreNullTransactionWithoutThrowing()
    {
        // Arrange, Act & Assert
        await _service.SaveTransactionAsync(null);
    }

    [Fact(DisplayName = "SaveTransactionAsync - Should handle repository exception")]
    public async Task CNABProcessingService_SaveTransactionAsync_ShouldHandleRepositoryException()
    {
        // Arrange
        var transaction = ServiceTestFactory.CreateTransaction();
        _transactionRepoMock.Setup(r => r.AddTransaction(transaction)).ThrowsAsync(new Exception("DB error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.SaveTransactionAsync(transaction));
    }

    [Fact(DisplayName = "ValidateLinesAsync - Should separate valid and invalid lines")]
    public async Task CNABProcessingService_ValidateLinesAsync_ShouldSeparateValidAndInvalidLines()
    {
        // Arrange
        var lines = new List<string> { ValidLinePadded, InvalidLine };

        // Act
        var (validLines, invalidLines) = await _service.ValidateLinesAsync(lines);

        // Assert
        Assert.Single(validLines);
        Assert.Contains(ValidLinePadded, validLines);
        Assert.Single(invalidLines);
        Assert.Contains(InvalidLine, invalidLines);
    }

    [Fact(DisplayName = "ValidateLinesAsync - Should return all lines as valid when all are valid")]
    public async Task CNABProcessingService_ValidateLinesAsync_ShouldReturnAllValidWhenAllLinesAreValid()
    {
        // Arrange
        var lines = new List<string> { ValidLinePadded, ValidLinePadded };

        // Act
        var (validLines, invalidLines) = await _service.ValidateLinesAsync(lines);

        // Assert
        Assert.Equal(2, validLines.Count);
        Assert.Empty(invalidLines);
    }

    [Fact(DisplayName = "ValidateLinesAsync - Should return all lines as invalid when all are invalid")]
    public async Task CNABProcessingService_ValidateLinesAsync_ShouldReturnAllInvalidWhenAllLinesAreInvalid()
    {
        // Arrange
        var lines = new List<string> { InvalidLine, InvalidLine };

        // Act
        var (validLines, invalidLines) = await _service.ValidateLinesAsync(lines);

        // Assert
        Assert.Empty(validLines);
        Assert.Equal(2, invalidLines.Count);
    }

    [Fact(DisplayName = "ValidateLinesAsync - Should handle empty list")]
    public async Task CNABProcessingService_ValidateLinesAsync_ShouldHandleEmptyList()
    {
        // Arrange
        var lines = new List<string>();

        // Act
        var (validLines, invalidLines) = await _service.ValidateLinesAsync(lines);

        // Assert
        Assert.Empty(validLines);
        Assert.Empty(invalidLines);
    }
}