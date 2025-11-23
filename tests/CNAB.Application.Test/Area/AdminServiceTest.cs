using CNAB.Application.DTOs;
using CNAB.Application.Interfaces;
using CNAB.Application.Services.Area;
using CNAB.TestHelpers.Factories;
using Moq;

namespace CNAB.Application.Test.Area;

public class AdminServiceTest
{
    private readonly Mock<IStoreService> _mockStoreService;
    private readonly Mock<ITransactionService> _mockTransactionService;
    private readonly AdminService _adminService;

    public AdminServiceTest()
    {
        _mockStoreService = new Mock<IStoreService>();
        _mockTransactionService = new Mock<ITransactionService>();
        _adminService = new AdminService(_mockStoreService.Object, _mockTransactionService.Object);
    }

    [Fact(DisplayName = "GetTotalBalanceAsync - With multiple Stores positive balance should return correct sum")]
    public async Task AdminService_GetTotalBalanceAsync_WithMultipleStoresPositiveBalanceShouldReturnCorrectSum()
    {
        // Arrange
        var stores = ServiceTestFactory.GenerateStoresWithPositiveBalances();
        _mockStoreService.Setup(x => x.GetAllStoreAsync()).ReturnsAsync(stores);

        // Act
        var result = await _adminService.GetTotalBalanceAsync();

        // Assert
        Assert.Equal(4251.50m, result);
        _mockStoreService.Verify(x => x.GetAllStoreAsync(), Times.Once);
    }

    [Fact(DisplayName = "GetTotalBalanceAsync - With mixed balances should return correct sum")]
    public async Task AdminService_GetTotalBalanceAsync_WithMixedBalancesShouldReturnCorrectSum()
    {
        // Arrange
        var stores = ServiceTestFactory.GenerateStoresWithPositiveandNegativeBalances();
        _mockStoreService.Setup(x => x.GetAllStoreAsync()).ReturnsAsync(stores);

        // Act
        var result = await _adminService.GetTotalBalanceAsync();

        // Assert
        Assert.Equal(750.00m, result);
        _mockStoreService.Verify(x => x.GetAllStoreAsync(), Times.Once);
    }

    [Fact(DisplayName = "GetTotalBalanceAsync - With all negative balances should return negative sum")]
    public async Task AdminService_GetTotalBalanceAsync_WithAllNegativeBalancesShouldReturnNegativeSum()
    {
        // Arrange
        var stores = ServiceTestFactory.GenerateStoresWithNegativeBalances();
        _mockStoreService.Setup(x => x.GetAllStoreAsync()).ReturnsAsync(stores);

        // Act
        var result = await _adminService.GetTotalBalanceAsync();

        // Assert
        Assert.Equal(-450.00m, result);
        _mockStoreService.Verify(x => x.GetAllStoreAsync(), Times.Once);
    }

    [Fact(DisplayName = "GetTotalBalanceAsync - With empty Store list should return zero")]
    public async Task AdminService_GetTotalBalanceAsync_WithEmptyStoreListShouldReturnZero()
    {
        // Arrange
        var stores = new List<StoreDto>();
        _mockStoreService.Setup(x => x.GetAllStoreAsync()).ReturnsAsync(stores);

        // Act
        var result = await _adminService.GetTotalBalanceAsync();

        // Assert
        Assert.Equal(0m, result);
        _mockStoreService.Verify(x => x.GetAllStoreAsync(), Times.Once);
    }

    [Fact(DisplayName = "GetTotalBalanceAsync - With single Store zero balance should return zero")]
    public async Task AdminService_GetTotalBalanceAsync_WithSingleStoreZeroBalanceShouldReturnZero()
    {
        // Arrange
        var stores = new List<StoreDto> { ServiceTestFactory.CreateStoreDto(Guid.NewGuid(), "Store 1", "Owner 1") };
        stores[0].Balance = 0.00m;
        _mockStoreService.Setup(x => x.GetAllStoreAsync()).ReturnsAsync(stores);

        // Act
        var result = await _adminService.GetTotalBalanceAsync();

        // Assert
        Assert.Equal(0m, result);
        _mockStoreService.Verify(x => x.GetAllStoreAsync(), Times.Once);
    }

    [Fact(DisplayName = "GetTotalBalanceAsync - When Store service throws exception should propagate exception")]
    public async Task AdminService_GetTotalBalanceAsync_WhenStoreServiceThrowsExceptionShouldPropagateException()
    {
        // Arrange
        _mockStoreService.Setup(x => x.GetAllStoreAsync())
                        .ThrowsAsync(new InvalidOperationException("Database connection failed"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _adminService.GetTotalBalanceAsync());

        Assert.Equal("Database connection failed", exception.Message);
        _mockStoreService.Verify(x => x.GetAllStoreAsync(), Times.Once);
    }

    [Fact(DisplayName = "GetStoreCountAsync - With multiple Stores should return correct count")]
    public async Task AdminService_GetStoreCountAsync_WithMultipleStoresShouldReturnCorrectCount()
    {
        // Arrange
        var stores = ServiceTestFactory.GenerateStoresWithPositiveBalances();
        _mockStoreService.Setup(x => x.GetAllStoreAsync()).ReturnsAsync(stores);

        // Act
        var result = await _adminService.GetStoreCountAsync();

        // Assert
        Assert.Equal(3, result);
        Assert.True(result >= 0, "Store count should never be negative");
        _mockStoreService.Verify(x => x.GetAllStoreAsync(), Times.Once);
    }

    [Fact(DisplayName = "GetStoreCountAsync - With empty Store list should return zero")]
    public async Task AdminService_GetStoreCountAsync_WithEmptyStoreListShouldReturnZero()
    {
        // Arrange
        var stores = new List<StoreDto>();
        _mockStoreService.Setup(x => x.GetAllStoreAsync()).ReturnsAsync(stores);

        // Act
        var result = await _adminService.GetStoreCountAsync();

        // Assert
        Assert.Equal(0, result);
        Assert.True(result >= 0, "Store count should never be negative");
        _mockStoreService.Verify(x => x.GetAllStoreAsync(), Times.Once);
    }

    [Fact(DisplayName = "GetStoreCountAsync - With single Store should return one")]
    public async Task AdminService_GetStoreCountAsync_WithSingleStoreShouldReturnOne()
    {
        // Arrange
        var stores = new List<StoreDto> { ServiceTestFactory.CreateStoreDto(Guid.NewGuid(), "Single Store", "Single Owner") };
        stores[0].Balance = 500.00m;
        _mockStoreService.Setup(x => x.GetAllStoreAsync()).ReturnsAsync(stores);

        // Act
        var result = await _adminService.GetStoreCountAsync();

        // Assert
        Assert.Equal(1, result);
        Assert.True(result >= 0, "Store count should never be negative");
        _mockStoreService.Verify(x => x.GetAllStoreAsync(), Times.Once);
    }

    [Fact(DisplayName = "GetStoreCountAsync - When Store service throws exception should propagate exception")]
    public async Task AdminService_GetStoreCountAsync_WhenStoreServiceThrowsExceptionShouldPropagateException()
    {
        // Arrange
        _mockStoreService.Setup(x => x.GetAllStoreAsync())
                        .ThrowsAsync(new InvalidOperationException("Service unavailable"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _adminService.GetStoreCountAsync());

        Assert.Equal("Service unavailable", exception.Message);
        _mockStoreService.Verify(x => x.GetAllStoreAsync(), Times.Once);
    }

    [Fact(DisplayName = "GetStoreCountAsync - With null Store list should throw exception")]
    public async Task AdminService_GetStoreCountAsync_WithNullStoreListShouldThrowException()
    {
        // Arrange
        _mockStoreService.Setup(x => x.GetAllStoreAsync()).ReturnsAsync((List<StoreDto>)null);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _adminService.GetStoreCountAsync());
        _mockStoreService.Verify(x => x.GetAllStoreAsync(), Times.Once);
    }

    [Fact(DisplayName = "GetTransactionCountAsync - With multiple Transactions should return correct count")]
    public async Task AdminService_GetTransactionCountAsync_WithMultipleTransactionsShouldReturnCorrectCount()
    {
        // Arrange
        var transactions = ServiceTestFactory.CreateTransactionList(5);
        _mockTransactionService.Setup(x => x.GetAllTransactionsAsync()).ReturnsAsync(transactions);

        // Act
        var result = await _adminService.GetTransactionCountAsync();

        // Assert
        Assert.Equal(5, result);
        Assert.True(result >= 0, "Transaction count should never be negative");
        _mockTransactionService.Verify(x => x.GetAllTransactionsAsync(), Times.Once);
    }

    [Fact(DisplayName = "GetTransactionCountAsync - With empty Transaction list should return zero")]
    public async Task AdminService_GetTransactionCountAsync_WithEmptyTransactionListShouldReturnZero()
    {
        // Arrange
        var transactions = new List<TransactionDto>();
        _mockTransactionService.Setup(x => x.GetAllTransactionsAsync()).ReturnsAsync(transactions);

        // Act
        var result = await _adminService.GetTransactionCountAsync();

        // Assert
        Assert.Equal(0, result);
        Assert.True(result >= 0, "Transaction count should never be negative");
        _mockTransactionService.Verify(x => x.GetAllTransactionsAsync(), Times.Once);
    }

    [Fact(DisplayName = "GetTransactionCountAsync - With single Transaction should return one")]
    public async Task AdminService_GetTransactionCountAsync_WithSingleTransactionShouldReturnOne()
    {
        // Arrange
        var transactions = new List<TransactionDto> { ServiceTestFactory.CreateTransactionDto(Guid.NewGuid()) };
        _mockTransactionService.Setup(x => x.GetAllTransactionsAsync()).ReturnsAsync(transactions);

        // Act
        var result = await _adminService.GetTransactionCountAsync();

        // Assert
        Assert.Equal(1, result);
        Assert.True(result >= 0, "Transaction count should never be negative");
        _mockTransactionService.Verify(x => x.GetAllTransactionsAsync(), Times.Once);
    }

    [Fact(DisplayName = "GetTransactionCountAsync - When Transaction service throws exception should propagate exception")]
    public async Task AdminService_GetTransactionCountAsync_WhenTransactionServiceThrowsExceptionShouldPropagateException()
    {
        // Arrange
        _mockTransactionService.Setup(x => x.GetAllTransactionsAsync())
                              .ThrowsAsync(new InvalidOperationException("Transaction service error"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _adminService.GetTransactionCountAsync());
        Assert.Equal("Transaction service error", exception.Message);
        _mockTransactionService.Verify(x => x.GetAllTransactionsAsync(), Times.Once);
    }

    [Fact(DisplayName = "GetTransactionCountAsync - With null Transaction list should throw exception")]
    public async Task AdminService_GetTransactionCountAsync_WithNullTransactionListShouldThrowException()
    {
        // Arrange
        _mockTransactionService.Setup(x => x.GetAllTransactionsAsync()).ReturnsAsync((List<TransactionDto>)null);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _adminService.GetTransactionCountAsync());
        _mockTransactionService.Verify(x => x.GetAllTransactionsAsync(), Times.Once);
    }

    [Fact(DisplayName = "AllMethods - Should work independently")]
    public async Task AdminService_AllMethods_ShouldWorkIndependently()
    {
        // Arrange
        var stores = ServiceTestFactory.GenerateStoresWithPositiveBalances();
        var transactions = ServiceTestFactory.CreateTransactionList(3);
        _mockStoreService.Setup(x => x.GetAllStoreAsync()).ReturnsAsync(stores);
        _mockTransactionService.Setup(x => x.GetAllTransactionsAsync()).ReturnsAsync(transactions);

        // Act
        var totalBalance = await _adminService.GetTotalBalanceAsync();
        var storeCount = await _adminService.GetStoreCountAsync();
        var transactionCount = await _adminService.GetTransactionCountAsync();

        // Assert
        Assert.Equal(4251.50m, totalBalance);
        Assert.Equal(3, storeCount);
        Assert.Equal(3, transactionCount);

        _mockStoreService.Verify(x => x.GetAllStoreAsync(), Times.Exactly(2));
        _mockTransactionService.Verify(x => x.GetAllTransactionsAsync(), Times.Once);
    }
}