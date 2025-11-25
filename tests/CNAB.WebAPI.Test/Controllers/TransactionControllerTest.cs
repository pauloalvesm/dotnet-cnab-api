using CNAB.Application.DTOs;
using CNAB.Application.Interfaces;
using CNAB.TestHelpers.Factories;
using CNAB.WebAPI.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CNAB.WebAPI.Test.Controllers;

public class TransactionControllerTest
{
    private readonly Mock<ITransactionService> _mockTransactionService;
    private readonly TransactionController _transactionController;

    public TransactionControllerTest()
    {
        _mockTransactionService = new Mock<ITransactionService>();
        _transactionController = new TransactionController(_mockTransactionService.Object);
    }

    [Fact(DisplayName = "GetAllTransactions - Should return all transactions when found")]
    public async Task TransactionController_GetAllTransactions_ShouldReturnAllTransactions()
    {
        // Arrange
        var transactions = ControllerTestFactory.GenerateListTransactions();
        _mockTransactionService.Setup(s => s.GetAllTransactionsAsync()).ReturnsAsync(transactions);

        // Act
        var result = await _transactionController.GetAllTransactions();

        // Assert
        result.Should().BeOfType<ActionResult<IEnumerable<TransactionDto>>>();
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedTransactions = okResult.Value.Should().BeAssignableTo<IEnumerable<TransactionDto>>().Subject;
        returnedTransactions.Should().NotBeNullOrEmpty().And.HaveCount(transactions.Count);
        returnedTransactions.Should().BeEquivalentTo(transactions);
    }

    [Fact(DisplayName = "GetAllTransactions - Should return NotFound when no transactions exist")]
    public async Task TransactionController_GetAllTransactions_ShouldReturnNotFoundWhenNoTransactionsExist()
    {
        // Arrange
        _mockTransactionService.Setup(s => s.GetAllTransactionsAsync()).ReturnsAsync(new List<TransactionDto>());

        // Act
        var result = await _transactionController.GetAllTransactions();

        // Assert
        result.Should().BeOfType<ActionResult<IEnumerable<TransactionDto>>>();
        var notFoundResult = result.Result.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFoundResult.Value.Should().Be("No transactions found.");
    }

    [Fact(DisplayName = "GetTransactionById - Should return a transaction when found")]
    public async Task TransactionController_GetTransactionById_ShouldReturnTransactionWhenFound()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var transaction = ControllerTestFactory.CreateTransactionDto();
        transaction.Id = transactionId;
        _mockTransactionService.Setup(s => s.GetTransactionByIdAsync(transactionId)).ReturnsAsync(transaction);

        // Act
        var result = await _transactionController.GetTransactionById(transactionId);

        // Assert
        result.Should().BeOfType<ActionResult<TransactionDto>>();
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedTransaction = okResult.Value.Should().BeOfType<TransactionDto>().Subject;
        returnedTransaction.Should().BeEquivalentTo(transaction);
    }

    [Fact(DisplayName = "GetTransactionById - Should return NotFound when transaction does not exist")]
    public async Task TransactionController_GetTransactionById_ShouldReturnNotFoundWhenTransactionDoesNotExist()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        _mockTransactionService.Setup(s => s.GetTransactionByIdAsync(transactionId)).ReturnsAsync((TransactionDto)null);

        // Act
        var result = await _transactionController.GetTransactionById(transactionId);

        // Assert
        result.Should().BeOfType<ActionResult<TransactionDto>>();
        var notFoundResult = result.Result.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFoundResult.Value.Should().Be("No transaction found.");
    }

    [Fact(DisplayName = "AddTransaction - Should create a new transaction")]
    public async Task TransactionController_AddTransaction_ShouldCreateNewTransaction()
    {
        // Arrange
        var transactionDtoInput = ControllerTestFactory.CreateTransactionDto();
        var createdTransactionDto = new TransactionDto
        {
            Id = Guid.NewGuid(),
            Type = transactionDtoInput.Type,
            OccurrenceDate = transactionDtoInput.OccurrenceDate,
            Amount = transactionDtoInput.Amount,
            CPF = transactionDtoInput.CPF,
            CardNumber = transactionDtoInput.CardNumber,
            Time = transactionDtoInput.Time,
            StoreId = transactionDtoInput.StoreId,
            StoreName = transactionDtoInput.StoreName,
            StoreOwnerName = transactionDtoInput.StoreOwnerName
        };

        _mockTransactionService.Setup(s => s.AddTransactionAsync(transactionDtoInput)).ReturnsAsync(createdTransactionDto);

        // Act
        var result = await _transactionController.AddTransaction(transactionDtoInput);

        // Assert
        result.Should().BeOfType<ActionResult<TransactionDto>>();
        var createdAtActionResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdAtActionResult.ActionName.Should().Be(nameof(TransactionController.GetTransactionById));
        createdAtActionResult.RouteValues["id"].Should().Be(createdTransactionDto.Id);
        createdAtActionResult.Value.Should().BeEquivalentTo(createdTransactionDto);
    }

    [Fact(DisplayName = "AddTransaction - Should return BadRequest when input is null")]
    public async Task TransactionController_AddTransaction_ShouldReturnBadRequestWhenInputIsNull()
    {
        // Arrange
        TransactionDto transactionDto = null;

        // Act
        var result = await _transactionController.AddTransaction(transactionDto);

        // Assert
        result.Result.Should().BeOfType<BadRequestResult>();
    }

    [Fact(DisplayName = "UpdateTransaction - Should update an existing transaction")]
    public async Task TransactionController_UpdateTransaction_ShouldUpdateExistingTransaction()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var updatedTransactionDto = ControllerTestFactory.UpdateTransactionDto(transactionId);
        _mockTransactionService.Setup(s => s.UpdateTransactionAsync(updatedTransactionDto)).ReturnsAsync(updatedTransactionDto);

        // Act
        var result = await _transactionController.UpdateTransaction(transactionId, updatedTransactionDto);

        // Assert
        result.Should().BeOfType<ActionResult<TransactionDto>>();
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedTransaction = okResult.Value.Should().BeOfType<TransactionDto>().Subject;
        returnedTransaction.Should().BeEquivalentTo(updatedTransactionDto);
    }

    [Fact(DisplayName = "UpdateTransaction - Should return BadRequest when input is null")]
    public async Task TransactionController_UpdateTransaction_ShouldReturnBadRequestWhenInputIsNull()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        TransactionDto transactionDto = null;

        // Act
        var result = await _transactionController.UpdateTransaction(transactionId, transactionDto);

        // Assert
        result.Result.Should().BeOfType<BadRequestResult>();
    }

    [Fact(DisplayName = "UpdateTransaction - Should return BadRequest when id mismatch")]
    public async Task TransactionController_UpdateTransaction_ShouldReturnBadRequestWhenIdMismatch()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var transactionDtoWithMismatchId = ControllerTestFactory.TransactionDtoNoId();

        // Act
        var result = await _transactionController.UpdateTransaction(transactionId, transactionDtoWithMismatchId);

        // Assert
        result.Result.Should().BeOfType<BadRequestResult>();
    }

    [Fact(DisplayName = "DeleteTransaction - Should delete a transaction successfully")]
    public async Task TransactionController_DeleteTransaction_ShouldDeleteTransactionSuccessfully()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var existingTransaction = ControllerTestFactory.GenerateListTransactions().First();
        existingTransaction.Id = transactionId;
        _mockTransactionService.Setup(s => s.GetTransactionByIdAsync(transactionId)).ReturnsAsync(existingTransaction);
        _mockTransactionService.Setup(s => s.DeleteTransactionAsync(transactionId)).Returns(Task.CompletedTask);

        // Act
        var result = await _transactionController.DeleteTransaction(transactionId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _mockTransactionService.Verify(s => s.DeleteTransactionAsync(transactionId), Times.Once);
    }

    [Fact(DisplayName = "DeleteTransaction - Should return NotFound when transaction does not exist")]
    public async Task TransactionController_DeleteTransaction_ShouldReturnNotFoundWhenTransactionDoesNotExist()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        _mockTransactionService.Setup(s => s.GetTransactionByIdAsync(transactionId)).ReturnsAsync((TransactionDto)null);

        // Act
        var result = await _transactionController.DeleteTransaction(transactionId);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
        ((NotFoundObjectResult)result).Value.Should().Be("No transaction found.");
        _mockTransactionService.Verify(s => s.DeleteTransactionAsync(It.IsAny<Guid>()), Times.Never);
    }
}