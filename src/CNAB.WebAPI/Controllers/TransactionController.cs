using CNAB.Application.DTOs;
using CNAB.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CNAB.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TransactionController : ControllerBase
{
    private readonly ITransactionService _transactionService;

    public TransactionController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TransactionDto>>> GetAllTransactions()
    {
        var transactions = await _transactionService.GetAllTransactionsAsync();
        return Ok(transactions);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TransactionDto>> GetTransactionById(Guid id)
    {
        var transaction = await _transactionService.GetTransactionByIdAsync(id);

        if (transaction == null)
        {
            return NotFound();
        }

        return Ok(transaction);
    }

    [HttpPost]
    public async Task<ActionResult<TransactionDto>> AddTransaction([FromBody] TransactionDto transactionDto)
    {
        if (transactionDto == null)
        {
            return BadRequest();
        }

        var createdTransaction = await _transactionService.AddTransactionAsync(transactionDto);
        return CreatedAtAction(nameof(GetTransactionById), new { id = createdTransaction.Id }, createdTransaction);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TransactionDto>> UpdateTransaction(Guid id, [FromBody] TransactionDto transactionDto)
    {
        if (transactionDto == null || id != transactionDto.Id)
        {
            return BadRequest();
        }

        var updatedTransaction = await _transactionService.UpdateTransactionAsync(transactionDto);
        return Ok(updatedTransaction);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTransaction(Guid id)
    {
        var transaction = await _transactionService.GetTransactionByIdAsync(id);

        if (transaction == null)
        {
            return NotFound();
        }

        await _transactionService.DeleteTransactionAsync(id);
        return NoContent();
    }
}