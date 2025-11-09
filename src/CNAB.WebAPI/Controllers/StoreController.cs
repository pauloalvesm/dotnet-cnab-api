using CNAB.Application.DTOs;
using CNAB.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CNAB.WebAPI.Controllers;

[Authorize(AuthenticationSchemes = "Bearer")]
[Route("api/[controller]")]
[ApiController]
public class StoreController : ControllerBase
{
    private readonly IStoreService _storeService;

    public StoreController(IStoreService storeService)
    {
        _storeService = storeService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<StoreDto>>> GetAllStores()
    {
        var stores = await _storeService.GetAllStoreAsync();

        if (stores == null || !stores.Any())
        {
            return NotFound("No stores found.");
        }

        return Ok(stores);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<StoreDto>> GetStoreById(Guid id)
    {
        var store = await _storeService.GetByIdStoreAsync(id);

        if (store == null)
        {
            return NotFound("No Store found.");
        }

        return Ok(store);
    }

    [HttpPost]
    public async Task<ActionResult<StoreDto>> AddStore([FromBody] StoreInputDto storeInputDto)
    {
        if (storeInputDto == null)
        {
            return BadRequest();
        }

        var createdStore = await _storeService.AddStoreAsync(storeInputDto);
        return CreatedAtAction(nameof(GetStoreById), new { id = createdStore.Id }, createdStore);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<StoreDto>> UpdateStore(Guid id, [FromBody] StoreInputDto storeInputDto)
    {
        if (storeInputDto == null || id != storeInputDto.Id)
        {
            return BadRequest();
        }

        var updatedStore = await _storeService.UpdateStoreAsync(storeInputDto);
        return Ok(updatedStore);
    }


    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminAccess")]
    public async Task<IActionResult> DeleteStore(Guid id)
    {
        var store = await _storeService.GetByIdStoreAsync(id);

        if (store == null)
        {
            return NotFound("No Store found.");
        }

        await _storeService.DeleteStoreAsync(id);

        return NoContent();
    }

}