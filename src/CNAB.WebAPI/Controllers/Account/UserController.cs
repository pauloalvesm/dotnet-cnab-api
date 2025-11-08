using CNAB.Application.DTOs.Account;
using CNAB.Application.Interfaces.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CNAB.WebAPI.Controllers.Account;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly ITokenService _tokenService;

    public UserController(UserManager<IdentityUser> userManager,
                          SignInManager<IdentityUser> signInManager,
                          ITokenService tokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<ActionResult> RegisterUser([FromBody] UserDto userDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = new IdentityUser
        {
            UserName = userDto.Email,
            Email = userDto.Email,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, userDto.Password);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return Ok(new { Message = "User registered successfully." });
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody] LoginDto loginDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await _userManager.FindByEmailAsync(loginDto.Email);

        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "User not found.");
            return BadRequest(ModelState);
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, lockoutOnFailure: false);

        if (result.Succeeded)
        {
            UserTokenDto token = _tokenService.GenerateToken(loginDto.Email);
            return Ok(token);
        }
        else
        {
            ModelState.AddModelError(string.Empty, "Invalid Login");
            return BadRequest(ModelState);
        }
    }
}