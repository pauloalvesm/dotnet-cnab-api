using CNAB.Application.DTOs;

namespace CNAB.Application.Interfaces;

public interface ITransactionService
{
    Task<IEnumerable<TransactionDto>> GetAllTransactionsAsync();
    Task<TransactionDto> GetTransactionByIdAsync(Guid id);
    Task<TransactionDto> AddTransactionAsync(TransactionDto transactionDto);
    Task<TransactionDto> UpdateTransactionAsync(TransactionDto transactionDto);
    Task DeleteTransactionAsync(Guid id);
}