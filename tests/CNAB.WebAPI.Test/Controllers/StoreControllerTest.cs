using CNAB.Application.DTOs;
using CNAB.Application.Interfaces;
using CNAB.TestHelpers.Factories;
using CNAB.WebAPI.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CNAB.WebAPI.Test.Controllers;

public class StoreControllerTest
{
    private readonly Mock<IStoreService> _mockStoreService;
    private readonly StoreController _storeController;

    public StoreControllerTest()
    {
        _mockStoreService = new Mock<IStoreService>();
        _storeController = new StoreController(_mockStoreService.Object);
    }

    [Fact(DisplayName = "GetAllStores - Return all Stores")]
    public async Task StoreController_GetAllStores_ShouldReturnAllStores()
    {
        // Arrange
        var stores = ControllerTestFactory.CreateStoreDtoList();
        _mockStoreService.Setup(s => s.GetAllStoreAsync()).ReturnsAsync(stores);

        // Act
        var result = await _storeController.GetAllStores();

        // Assert
        result.Should().BeOfType<ActionResult<IEnumerable<StoreDto>>>();
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedStores = okResult.Value.Should().BeAssignableTo<IEnumerable<StoreDto>>().Subject;
        returnedStores.Should().NotBeNullOrEmpty().And.HaveCount(stores.Count);
        returnedStores.Should().BeEquivalentTo(stores);
    }

    [Fact(DisplayName = "GetAllStores - Should return empty list when no Stores exist")]
    public async Task StoreController_GetAllStores_ShouldReturnEmptyListWhenNoStoresExist()
    {
        // Arrange
        _mockStoreService.Setup(s => s.GetAllStoreAsync()).ReturnsAsync(new List<StoreDto>());

        // Act
        var result = await _storeController.GetAllStores();

        // Assert
        result.Should().BeOfType<ActionResult<IEnumerable<StoreDto>>>();
        var notFoundResult = result.Result.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFoundResult.Value.Should().Be("No stores found.");
    }

    [Fact(DisplayName = "GetStoreById - Should return a Store when found")]
    public async Task StoreController_GetStoreById_ShouldReturnStoreWhenFound()
    {
        // Arrange
        var storeId = new Guid("218bc11d-6167-4166-85a0-28842e1ab4bf");
        var store = ControllerTestFactory.CreateStoreDto(storeId);

        _mockStoreService.Setup(s => s.GetByIdStoreAsync(storeId)).ReturnsAsync(store);

        // Act
        var result = await _storeController.GetStoreById(storeId);

        // Assert
        result.Should().BeOfType<ActionResult<StoreDto>>();
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedStore = okResult.Value.Should().BeOfType<StoreDto>().Subject;
        returnedStore.Should().BeEquivalentTo(store);
    }

    [Fact(DisplayName = "GetStoreById - Should return NotFound when Store does not exist")]
    public async Task StoreController_GetStoreById_ShouldReturnNotFoundWhenStoreDoesNotExist()
    {
        // Arrange
        var storeId = Guid.NewGuid();
        _mockStoreService.Setup(s => s.GetByIdStoreAsync(storeId)).ReturnsAsync((StoreDto)null);

        // Act
        var result = await _storeController.GetStoreById(storeId);

        // Assert
        result.Should().BeOfType<ActionResult<StoreDto>>();
        var notFoundResult = result.Result.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFoundResult.Value.Should().Be("No Store found.");
    }

    [Fact(DisplayName = "AddStore - Should create a new Store")]
    public async Task StoreController_AddStore_ShouldCreateNewStore()
    {
        // Arrange
        var storeInputDto = ControllerTestFactory.CreateStoreInputDto();
        var createdStoreDto = new StoreDto(Guid.NewGuid(), storeInputDto.Name, storeInputDto.OwnerName, 0m);
        _mockStoreService.Setup(s => s.AddStoreAsync(storeInputDto)).ReturnsAsync(createdStoreDto);

        // Act
        var result = await _storeController.AddStore(storeInputDto);

        // Assert
        result.Should().BeOfType<ActionResult<StoreDto>>();
        var createdAtActionResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdAtActionResult.ActionName.Should().Be(nameof(StoreController.GetStoreById));
        createdAtActionResult.RouteValues["id"].Should().Be(createdStoreDto.Id);
        createdAtActionResult.Value.Should().BeEquivalentTo(createdStoreDto);
    }

    [Fact(DisplayName = "AddStore - Should return BadRequest when input is null")]
    public async Task StoreController_AddStore_ShouldReturnBadRequestWhenInputIsNull()
    {
        // Arrange
        StoreInputDto storeInputDto = null;

        // Act
        var result = await _storeController.AddStore(storeInputDto);

        // Assert
        result.Result.Should().BeOfType<BadRequestResult>();
    }

    [Fact(DisplayName = "UpdateStore - Should update an existing Store")]
    public async Task StoreController_UpdateStore_ShouldUpdateExistingStore()
    {
        // Arrange
        var storeId = new Guid("218bc11d-6167-4166-85a0-28842e1ab4bf");
        var updatedStoreInputDto = ControllerTestFactory.CreateStoreInputDto(storeId);
        var updatedStoreDto = new StoreDto(updatedStoreInputDto.Id, updatedStoreInputDto.Name, updatedStoreInputDto.OwnerName, 1500m);

        _mockStoreService.Setup(s => s.UpdateStoreAsync(updatedStoreInputDto)).ReturnsAsync(updatedStoreDto);

        // Act
        var result = await _storeController.UpdateStore(storeId, updatedStoreInputDto);

        // Assert
        result.Should().BeOfType<ActionResult<StoreDto>>();
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedStore = okResult.Value.Should().BeOfType<StoreDto>().Subject;
        returnedStore.Should().BeEquivalentTo(updatedStoreDto);
    }

    [Fact(DisplayName = "UpdateStore - Should return BadRequest when input is null")]
    public async Task StoreController_UpdateStore_ShouldReturnBadRequestWhenInputIsNull()
    {
        // Arrange
        var storeId = Guid.NewGuid();
        StoreInputDto storeInputDto = null;

        // Act
        var result = await _storeController.UpdateStore(storeId, storeInputDto);

        // Assert
        result.Result.Should().BeOfType<BadRequestResult>();
    }

    [Fact(DisplayName = "UpdateStore - Should return BadRequest when id mismatch")]
    public async Task StoreController_UpdateStore_ShouldReturnBadRequestWhenIdMismatch()
    {
        // Arrange
        var storeId = Guid.NewGuid();
        var storeInputDtoWithMismatchId = ControllerTestFactory.CreateStoreInputDto(Guid.NewGuid());

        // Act
        var result = await _storeController.UpdateStore(storeId, storeInputDtoWithMismatchId);

        // Assert
        result.Result.Should().BeOfType<BadRequestResult>();
    }

    [Fact(DisplayName = "DeleteStore - Should delete a Store successfully")]
    public async Task StoreController_DeleteStore_ShouldDeleteStoreSuccessfully()
    {
        // Arrange
        var storeId = new Guid("218bc11d-6167-4166-85a0-28842e1ab4bf");
        var existingStore = ControllerTestFactory.CreateStoreDto(storeId);

        _mockStoreService.Setup(s => s.GetByIdStoreAsync(storeId)).ReturnsAsync(existingStore);
        _mockStoreService.Setup(s => s.DeleteStoreAsync(storeId)).Returns(Task.CompletedTask);

        // Act
        var result = await _storeController.DeleteStore(storeId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _mockStoreService.Verify(s => s.DeleteStoreAsync(storeId), Times.Once);
    }

    [Fact(DisplayName = "DeleteStore - Should return NotFound when Store does not exist")]
    public async Task StoreController_DeleteStore_ShouldReturnNotFoundWhenStoreDoesNotExist()
    {
        // Arrange
        var storeId = Guid.NewGuid();
        _mockStoreService.Setup(s => s.GetByIdStoreAsync(storeId)).ReturnsAsync((StoreDto)null);

        // Act
        var result = await _storeController.DeleteStore(storeId);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
        ((NotFoundObjectResult)result).Value.Should().Be("No Store found.");
        _mockStoreService.Verify(s => s.DeleteStoreAsync(It.IsAny<Guid>()), Times.Never);
    }
}