using CNAB.Application.Interfaces.Area;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CNAB.WebAPI.Controllers.Area;

[Authorize(AuthenticationSchemes = "Bearer", Policy = "AdminAccess")]
[Route("api/[controller]")]
[ApiController]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpGet("total-balance")]
    public async Task<IActionResult> GetTotalBalance()
    {
        var totalBalance = await _adminService.GetTotalBalanceAsync();
        return Ok(new { totalBalance });
    }

    [HttpGet("store-count")]
    public async Task<IActionResult> GetStoreCount()
    {
        var count = await _adminService.GetStoreCountAsync();

        if (count == 0)
        {
            return NotFound("No stores found.");
        }

        return Ok(new { count });
    }

    [HttpGet("transaction-count")]
    public async Task<IActionResult> GetTransactionCount()
    {
        var count = await _adminService.GetTransactionCountAsync();

        if (count == 0)
        {
            return NotFound("No transactions found.");
        }

        return Ok(new { count });
    }
}