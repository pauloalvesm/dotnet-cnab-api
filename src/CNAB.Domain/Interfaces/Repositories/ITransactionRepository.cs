using CNAB.Domain.Entities;

namespace CNAB.Domain.Interfaces.Repositories;

public interface ITransactionRepository
{
    Task<IEnumerable<Transaction>> GetAllTransactions();
    Task<Transaction> GetTransactionById(Guid id);
    Task<Transaction> AddTransaction(Transaction transaction);
    Task<Transaction> UpdateTransaction(Transaction transaction);
    Task DeleteTransaction(Guid id);
}