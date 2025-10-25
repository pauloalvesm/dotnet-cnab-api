using CNAB.Domain.Entities;
using CNAB.Domain.Interfaces.Repositories;
using CNAB.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CNAB.Infra.Data.Repositories;

public class TransactionRepository : ITransactionRepository
{
    protected readonly ApplicationDbContext _transactionContext;
    protected readonly ILogger<TransactionRepository> _logger;

    public TransactionRepository(ApplicationDbContext transactionContext, ILogger<TransactionRepository> logger)
    {
        _transactionContext = transactionContext;
        _logger = logger;
    }

    public async Task<IEnumerable<Transaction>> GetAllTransactions()
    {
        var transactions = await _transactionContext.Set<Transaction>()
            .Include(s => s.Store)
            .ToListAsync();

        if (transactions == null || !transactions.Any())
        {
            _logger.LogWarning("No transactions found in the database.");
            return Enumerable.Empty<Transaction>();
        }

        return transactions;
    }

    public async Task<Transaction> GetTransactionById(Guid id)
    {
        var transaction = await _transactionContext.Set<Transaction>()
            .Include(s => s.Store)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (transaction == null)
        {
            _logger.LogWarning($"No transaction found with ID: {id}");
        }

        return transaction;
    }

    public async Task<Transaction> AddTransaction(Transaction transaction)
    {
        try
        {
            _transactionContext.Set<Transaction>().Add(transaction);
            await _transactionContext.SaveChangesAsync();
            return transaction;
        }
        catch (DbUpdateException exception)
        {
            _logger.LogError($"Error when adding a new transaction: {exception.Message}");
            throw;
        }
    }

    public async Task<Transaction> UpdateTransaction(Transaction transaction)
    {
        try
        {
            _transactionContext.Update(transaction);
            await _transactionContext.SaveChangesAsync();
            return transaction;
        }
        catch (DbUpdateException exception)
        {
            _logger.LogError($"Error when updating the transaction: {exception.Message}");
            throw;
        }
    }
    
    public async Task DeleteTransaction(Guid id)
    {
        try
        {
            var transaction = await _transactionContext.Set<Transaction>().FindAsync(id);

            if (transaction != null)
            {
                _transactionContext.Set<Transaction>().Remove(transaction);
                await _transactionContext.SaveChangesAsync();
            }
        }
        catch (DbUpdateException exception)
        {
            _logger.LogError($"Error when deleting the transaction: {exception.Message}");
            throw;
        }
    }
}