using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CNAB.WebAPI.Controllers.Account;
using CNAB.Application.DTOs.Account;
using CNAB.Application.Interfaces.Account;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;
using CNAB.TestHelpers.Factories;

namespace CNAB.WebAPI.Test.Account;

public class UserControllerTest
{
    private readonly Mock<UserManager<IdentityUser>> _userManagerMock;
    private readonly Mock<SignInManager<IdentityUser>> _signInManagerMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly UserController _controller;

    public UserControllerTest()
    {
        _userManagerMock = ControllerTestFactory.MockUserManager();
        _signInManagerMock = ControllerTestFactory.MockSignInManager(_userManagerMock);
        _tokenServiceMock = new Mock<ITokenService>();

        _controller = new UserController(
            _userManagerMock.Object,
            _signInManagerMock.Object,
            _tokenServiceMock.Object
        );
    }

    [Fact(DisplayName = "RegisterUser - Should return ok when User is created")]
    public async Task UserController_RegisterUser_ShouldReturnOkWhenUserIsCreated()
    {
        // Arrange
        var dto = new UserDto { Email = "test@example.com", Password = "Pass@123" };
        _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _controller.RegisterUser(dto);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact(DisplayName = "RegisterUser - Should return BadRequest when creation fails")]
    public async Task UserController_RegisterUser_ShouldReturnBadRequestWhenCreationFails()
    {
        // Arrange
        var dto = new UserDto { Email = "fail@example.com", Password = "Pass@123" };
        var identityResult = IdentityResult.Failed(new IdentityError { Description = "Creation failed." });

        _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
            .ReturnsAsync(identityResult);

        // Act
        var result = await _controller.RegisterUser(dto);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact(DisplayName = "Login - Should return ok when credentials are valid")]
    public async Task UserController_Login_ShouldReturnOkWhenCredentialsAreValid()
    {
        // Arrange
        var loginDto = new LoginDto { Email = "valid@example.com", Password = "Pass@123" };
        var user = new IdentityUser { UserName = loginDto.Email, Email = loginDto.Email };

        _userManagerMock.Setup(x => x.FindByEmailAsync(loginDto.Email)).ReturnsAsync(user);
        _signInManagerMock.Setup(x => x.CheckPasswordSignInAsync(user, loginDto.Password, false))
            .ReturnsAsync(SignInResult.Success);
        _tokenServiceMock.Setup(x => x.GenerateToken(loginDto.Email))
            .Returns(new UserTokenDto { Token = "fake-token" });

        // Act
        var result = await _controller.Login(loginDto);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact(DisplayName = "Login - Should return BadRequest when User not found")]
    public async Task UserController_Login_ShouldReturnBadRequestWhenUserNotFound()
    {
        // Arrange
        var loginDto = new LoginDto { Email = "notfound@example.com", Password = "Pass@123" };
        _userManagerMock.Setup(x => x.FindByEmailAsync(loginDto.Email)).ReturnsAsync((IdentityUser)null);

        // Act
        var result = await _controller.Login(loginDto);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact(DisplayName = "Login - Should return BadRequest when password is incorrect")]
    public async Task UserController_Login_ShouldReturnBadRequestWhenPasswordIsIncorrect()
    {
        // Arrange
        var loginDto = new LoginDto { Email = "wrongpass@example.com", Password = "WrongPass" };
        var user = new IdentityUser { Email = loginDto.Email };

        _userManagerMock.Setup(x => x.FindByEmailAsync(loginDto.Email)).ReturnsAsync(user);
        _signInManagerMock.Setup(x => x.CheckPasswordSignInAsync(user, loginDto.Password, false))
            .ReturnsAsync(SignInResult.Failed);

        // Act
        var result = await _controller.Login(loginDto);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }
}