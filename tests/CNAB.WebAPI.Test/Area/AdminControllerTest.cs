using CNAB.Application.Interfaces.Area;
using CNAB.WebAPI.Controllers.Area;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CNAB.WebAPI.Test.Area;

public class AdminControllerTest
{
    private readonly Mock<IAdminService> _mockAdminService;
    private readonly AdminController _adminController;

    public AdminControllerTest()
    {
        _mockAdminService = new Mock<IAdminService>();
        _adminController = new AdminController(_mockAdminService.Object);
    }

    [Fact(DisplayName = "GetStoreCount - When called should return ok with total balance")]
    public async Task AdminController_GetTotalBalance_WhenCalledShouldReturnOkWithTotalBalance()
    {
        // Arrange
        var expectedBalance = 5000.50m;
        _mockAdminService.Setup(x => x.GetTotalBalanceAsync()).ReturnsAsync(expectedBalance);

        // Act
        var result = await _adminController.GetTotalBalance();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult.StatusCode.Should().Be(200);

        var responseValue = okResult.Value;
        responseValue.Should().NotBeNull();
        responseValue.GetType().GetProperty("totalBalance")?.GetValue(responseValue).Should().Be(expectedBalance);
        _mockAdminService.Verify(x => x.GetTotalBalanceAsync(), Times.Once);
    }

    [Fact(DisplayName = "GetStoreCount - When balance is zero should return ok with zero balance")]
    public async Task AdminController_GetTotalBalance_WhenBalanceIsZeroShouldReturnOkWithZeroBalance()
    {
        // Arrange
        var expectedBalance = 0m;
        _mockAdminService.Setup(x => x.GetTotalBalanceAsync()).ReturnsAsync(expectedBalance);

        // Act
        var result = await _adminController.GetTotalBalance();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult.StatusCode.Should().Be(200);

        var responseValue = okResult.Value;
        responseValue.GetType().GetProperty("totalBalance")?.GetValue(responseValue).Should().Be(expectedBalance);
        _mockAdminService.Verify(x => x.GetTotalBalanceAsync(), Times.Once);
    }

    [Fact(DisplayName = "GetStoreCount - When service throws exception should propagate exception")]
    public async Task AdminController_GetTotalBalance_WhenServiceThrowsExceptionShouldPropagateException()
    {
        // Arrange
        var expectedException = new InvalidOperationException("Database connection failed");
        _mockAdminService.Setup(x => x.GetTotalBalanceAsync()).ThrowsAsync(expectedException);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _adminController.GetTotalBalance());
        exception.Message.Should().Be("Database connection failed");
        _mockAdminService.Verify(x => x.GetTotalBalanceAsync(), Times.Once);
    }

    [Fact(DisplayName = "GetStoreCount - When Stores exist should return ok with count")]
    public async Task AdminController_GetStoreCount_WhenStoresExistShouldReturnOkWithCount()
    {
        // Arrange
        var expectedCount = 15;
        _mockAdminService.Setup(x => x.GetStoreCountAsync()).ReturnsAsync(expectedCount);

        // Act
        var result = await _adminController.GetStoreCount();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult.StatusCode.Should().Be(200);

        var responseValue = okResult.Value;
        responseValue.Should().NotBeNull();
        responseValue.GetType().GetProperty("count")?.GetValue(responseValue).Should().Be(expectedCount);
        _mockAdminService.Verify(x => x.GetStoreCountAsync(), Times.Once);
    }

    [Fact(DisplayName = "GetStoreCount - When no Stores exist should return not found")]
    public async Task AdminController_GetStoreCount_WhenNoStoresExistShouldReturnNotFound()
    {
        // Arrange
        var expectedCount = 0;
        _mockAdminService.Setup(x => x.GetStoreCountAsync()).ReturnsAsync(expectedCount);

        // Act
        var result = await _adminController.GetStoreCount();

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult.StatusCode.Should().Be(404);
        notFoundResult.Value.Should().Be("No stores found.");
        _mockAdminService.Verify(x => x.GetStoreCountAsync(), Times.Once);
    }

    [Fact(DisplayName = "GetStoreCount - When service throws exception should propagate exception")]
    public async Task AdminController_GetStoreCount_WhenServiceThrowsExceptionShouldPropagateException()
    {
        // Arrange
        var expectedException = new ArgumentException("Invalid operation");
        _mockAdminService.Setup(x => x.GetStoreCountAsync()).ThrowsAsync(expectedException);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _adminController.GetStoreCount());
        exception.Message.Should().Be("Invalid operation");
        _mockAdminService.Verify(x => x.GetStoreCountAsync(), Times.Once);
    }

    [Theory(DisplayName = "GetStoreCount - With different positive counts should return ok with correct count")]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(100)]
    [InlineData(999)]
    public async Task AdminController_GetStoreCount_WithDifferentPositiveCountsShouldReturnOkWithCorrectCount(int count)
    {
        // Arrange
        _mockAdminService.Setup(x => x.GetStoreCountAsync()).ReturnsAsync(count);

        // Act
        var result = await _adminController.GetStoreCount();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult.StatusCode.Should().Be(200);

        var responseValue = okResult.Value;
        responseValue.GetType().GetProperty("count")?.GetValue(responseValue).Should().Be(count);
        _mockAdminService.Verify(x => x.GetStoreCountAsync(), Times.Once);
    }

    [Fact(DisplayName = "GetTransactionCount - When Transactions exist should return ok with count")]
    public async Task AdminController_GetTransactionCount_WhenTransactionsExistShouldReturnOkWithCount()
    {
        // Arrange
        var expectedCount = 250;
        _mockAdminService.Setup(x => x.GetTransactionCountAsync()).ReturnsAsync(expectedCount);

        // Act
        var result = await _adminController.GetTransactionCount();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult.StatusCode.Should().Be(200);

        var responseValue = okResult.Value;
        responseValue.Should().NotBeNull();
        responseValue.GetType().GetProperty("count")?.GetValue(responseValue).Should().Be(expectedCount);
        _mockAdminService.Verify(x => x.GetTransactionCountAsync(), Times.Once);
    }

    [Fact(DisplayName = "GetTransactionCount - When no Transactions exist should return not found")]
    public async Task AdminController_GetTransactionCount_WhenNoTransactionsExistShouldReturnNotFound()
    {
        // Arrange
        var expectedCount = 0;
        _mockAdminService.Setup(x => x.GetTransactionCountAsync()).ReturnsAsync(expectedCount);

        // Act
        var result = await _adminController.GetTransactionCount();

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult.StatusCode.Should().Be(404);
        notFoundResult.Value.Should().Be("No transactions found.");

        _mockAdminService.Verify(x => x.GetTransactionCountAsync(), Times.Once);
    }

    [Fact(DisplayName = "GetTransactionCount - When service throws exception should propagate exception")]
    public async Task AdminController_GetTransactionCount_WhenServiceThrowsExceptionShouldPropagateException()
    {
        // Arrange
        var expectedException = new TimeoutException("Request timeout");
        _mockAdminService.Setup(x => x.GetTransactionCountAsync()).ThrowsAsync(expectedException);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<TimeoutException>(() => _adminController.GetTransactionCount());

        exception.Message.Should().Be("Request timeout");
        _mockAdminService.Verify(x => x.GetTransactionCountAsync(), Times.Once);
    }

    [Theory(DisplayName = "GetTransactionCount - With different positive counts should return ok with correct Count")]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(1000)]
    [InlineData(50000)]
    public async Task AdminController_GetTransactionCount_WithDifferentPositiveCountsShouldReturnOkWithCorrectCount(int count)
    {
        // Arrange
        _mockAdminService.Setup(x => x.GetTransactionCountAsync()).ReturnsAsync(count);

        // Act
        var result = await _adminController.GetTransactionCount();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult.StatusCode.Should().Be(200);

        var responseValue = okResult.Value;
        responseValue.GetType().GetProperty("count")?.GetValue(responseValue).Should().Be(count);
        _mockAdminService.Verify(x => x.GetTransactionCountAsync(), Times.Once);
    }

    [Fact(DisplayName = "Constructor - When admin service is null should accept null and create instance")]
    public void AdminController_Constructor_WhenAdminServiceIsNullShouldAcceptNullAndCreateInstance()
    {
        // Act
        var controller = new AdminController(null);

        // Assert
        controller.Should().NotBeNull();
        controller.Should().BeOfType<AdminController>();
    }

    [Fact(DisplayName = "Constructor - When admin service is provided should create instance")]
    public void AdminController_Constructor_WhenAdminServiceIsProvidedShouldCreateInstance()
    {
        // Arrange
        var mockService = new Mock<IAdminService>();

        // Act
        var controller = new AdminController(mockService.Object);

        // Assert
        controller.Should().NotBeNull();
        controller.Should().BeOfType<AdminController>();
    }

    [Fact(DisplayName = "Methods - When admin service is null should throw null reference exception")]
    public async Task AdminController_Methods_WhenAdminServiceIsNullShouldThrowNullReferenceException()
    {
        // Arrange
        var controller = new AdminController(null);

        // Act & Assert
        await Assert.ThrowsAsync<NullReferenceException>(() => controller.GetTotalBalance());
        await Assert.ThrowsAsync<NullReferenceException>(() => controller.GetStoreCount());
        await Assert.ThrowsAsync<NullReferenceException>(() => controller.GetTransactionCount());
    }

    [Fact(DisplayName = "GetAllCounts - When all methods called should return consistent results")]
    public async Task AdminController_GetAllCounts_WhenAllMethodsCalledShouldReturnConsistentResults()
    {
        // Arrange
        var expectedStoreCount = 5;
        var expectedTransactionCount = 25;
        var expectedTotalBalance = 12500.75m;

        _mockAdminService.Setup(x => x.GetStoreCountAsync()).ReturnsAsync(expectedStoreCount);
        _mockAdminService.Setup(x => x.GetTransactionCountAsync()).ReturnsAsync(expectedTransactionCount);
        _mockAdminService.Setup(x => x.GetTotalBalanceAsync()).ReturnsAsync(expectedTotalBalance);

        // Act
        var storeResult = await _adminController.GetStoreCount();
        var transactionResult = await _adminController.GetTransactionCount();
        var balanceResult = await _adminController.GetTotalBalance();

        // Assert
        storeResult.Should().BeOfType<OkObjectResult>();
        transactionResult.Should().BeOfType<OkObjectResult>();
        balanceResult.Should().BeOfType<OkObjectResult>();

        _mockAdminService.Verify(x => x.GetStoreCountAsync(), Times.Once);
        _mockAdminService.Verify(x => x.GetTransactionCountAsync(), Times.Once);
        _mockAdminService.Verify(x => x.GetTotalBalanceAsync(), Times.Once);
    }

    [Fact(DisplayName = "GetCounts - When systemIs empty should return appropriate responses")]
    public async Task AdminController_GetCounts_WhenSystemIsEmptyShouldReturnAppropriateResponses()
    {
        // Arrange
        _mockAdminService.Setup(x => x.GetStoreCountAsync()).ReturnsAsync(0);
        _mockAdminService.Setup(x => x.GetTransactionCountAsync()).ReturnsAsync(0);
        _mockAdminService.Setup(x => x.GetTotalBalanceAsync()).ReturnsAsync(0m);

        // Act
        var storeResult = await _adminController.GetStoreCount();
        var transactionResult = await _adminController.GetTransactionCount();
        var balanceResult = await _adminController.GetTotalBalance();

        // Assert
        storeResult.Should().BeOfType<NotFoundObjectResult>();
        transactionResult.Should().BeOfType<NotFoundObjectResult>();
        balanceResult.Should().BeOfType<OkObjectResult>();

        var storeNotFound = storeResult as NotFoundObjectResult;
        var transactionNotFound = transactionResult as NotFoundObjectResult;
        var balanceOk = balanceResult as OkObjectResult;

        storeNotFound.Value.Should().Be("No stores found.");
        transactionNotFound.Value.Should().Be("No transactions found.");
        balanceOk.Value.GetType().GetProperty("totalBalance")?.GetValue(balanceOk.Value).Should().Be(0m);
    }
}