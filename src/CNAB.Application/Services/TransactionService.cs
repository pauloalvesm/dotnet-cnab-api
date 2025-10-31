using CNAB.Application.DTOs;
using CNAB.Application.Interfaces;
using CNAB.Domain.Entities;
using CNAB.Domain.Entities.enums;
using CNAB.Domain.Interfaces.Repositories;
using MapsterMapper;
using Microsoft.Extensions.Logging;

namespace CNAB.Application.Services;

public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IStoreRepository _storeRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<TransactionService> _logger;

    public TransactionService(ITransactionRepository transactionRepository,
                              IStoreRepository storeRepository,
                              IMapper mapper,
                              ILogger<TransactionService> logger)
    {
        _transactionRepository = transactionRepository;
        _storeRepository = storeRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<TransactionDto>> GetAllTransactionsAsync()
    {
        var transactions = await _transactionRepository.GetAllTransactions();
        return _mapper.Map<IEnumerable<TransactionDto>>(transactions);
    }

    public async Task<TransactionDto> GetTransactionByIdAsync(Guid id)
    {
        var transaction = await _transactionRepository.GetTransactionById(id);

        if (transaction == null)
        {
            _logger.LogError($"Transaction not found");
            return null;
        }

        return _mapper.Map<TransactionDto>(transaction);
    }

    public async Task<TransactionDto> AddTransactionAsync(TransactionDto transactionDto)
    {
        var store = await _storeRepository.GetStoreById(transactionDto.StoreId);

        if (store == null)
        {
            _logger.LogError("Store not found");
            return null;
        }

        var transaction = new Transaction(
            (TransactionType)transactionDto.Type,
            transactionDto.OccurrenceDate,
            transactionDto.Amount,
            transactionDto.CPF,
            transactionDto.CardNumber,
            transactionDto.Time,
            store
        );

        await _transactionRepository.AddTransaction(transaction);
        return _mapper.Map<TransactionDto>(transaction);
    }

    public async Task<TransactionDto> UpdateTransactionAsync(TransactionDto transactionDto)
    {
        var existingTransaction = await _transactionRepository.GetTransactionById(transactionDto.Id);

        if (existingTransaction == null)
        {
            _logger.LogError("Transaction not found");
            return null;
        }

        existingTransaction.UpdateDetails(
            (TransactionType)transactionDto.Type,
            transactionDto.OccurrenceDate,
            transactionDto.Amount,
            transactionDto.CPF,
            transactionDto.CardNumber,
            transactionDto.Time
        );

        await _transactionRepository.UpdateTransaction(existingTransaction);
        return _mapper.Map<TransactionDto>(existingTransaction);
    }

    public async Task DeleteTransactionAsync(Guid id)
    {
        var existingTransaction = await _transactionRepository.GetTransactionById(id);

        if (existingTransaction == null)
        {
            _logger.LogError("Transaction not found");
        }

        await _transactionRepository.DeleteTransaction(id);
    }
}