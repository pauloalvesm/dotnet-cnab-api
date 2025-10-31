using CNAB.Application.DTOs;
using CNAB.Application.Interfaces;
using CNAB.Domain.Entities;
using CNAB.Domain.Entities.enums;
using CNAB.Domain.Interfaces.Repositories;
using MapsterMapper;

namespace CNAB.Application.Services;

public class CNABProcessingService : ICNABProcessingService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IStoreRepository _storeRepository;
    private readonly IMapper _mapper;

    public CNABProcessingService(ITransactionRepository transactionRepository, IStoreRepository storeRepository, IMapper mapper)
    {
        _transactionRepository = transactionRepository;
        _storeRepository = storeRepository;
        _mapper = mapper;
    }

    public async Task<ParseResultDto> ParseCNABAsync(IEnumerable<string> lines)
    {
        var (validLines, invalidLines) = await ValidateLinesAsync(lines);

        if (invalidLines.Count > 0)
        {
            return new ParseResultDto
            {
                Success = false,
                TotalProcessed = 0,
                Message = $"Invalid line lengths detected in {invalidLines.Count} lines."
            };
        }

        var totalProcessed = await ProcessValidLinesAsync(validLines);

        return new ParseResultDto
        {
            Success = true,
            TotalProcessed = totalProcessed,
            Message = "File processed successfully."
        };
    }

    public async Task<int> ProcessValidLinesAsync(IEnumerable<string> validLines)
    {
        var totalProcessed = 0;

        foreach (var line in validLines)
        {
            var transaction = await ParseLineAsync(line);
            await SaveTransactionAsync(transaction);
            totalProcessed++;
        }

        return totalProcessed;
    }

    public async Task<Transaction> ParseLineAsync(string line)
    {
        var type = (TransactionType)int.Parse(line.Substring(0, 1));
        var date = DateTime.ParseExact(line.Substring(1, 8), "yyyyMMdd", null);
        var value = decimal.Parse(line.Substring(9, 10)) / 100.0m;
        var cpf = line.Substring(19, 11).Trim();
        var card = line.Substring(30, 12).Trim();
        var time = TimeSpan.ParseExact(line.Substring(42, 6), "hhmmss", null);
        var owner = line.Substring(48, 14).Trim();
        var storeName = line.Substring(62, 19).Trim();

        var dateTime = date.Add(time);
        var store = await GetOrCreateStoreAsync(storeName, owner);

        return new Transaction(type, dateTime, value, cpf, card, time, store);
    }

    public async Task<Store> GetOrCreateStoreAsync(string storeName, string owner)
    {
        var store = await _storeRepository.GetStoreByName(storeName);

        if (store == null)
        {
            store = new Store(storeName, owner);
            await _storeRepository.AddStore(store);
        }

        return store;
    }

    public async Task SaveTransactionAsync(Transaction transaction)
    {
        await _transactionRepository.AddTransaction(transaction);
    }

    public Task<(List<string> validLines, List<string> invalidLines)> ValidateLinesAsync(IEnumerable<string> lines)
    {
        var validLines = new List<string>();
        var invalidLines = new List<string>();

        foreach (var line in lines)
        {
            if (line.Length != 81)
            {
                invalidLines.Add(line);
            }
            else
            {
                validLines.Add(line);
            }
        }

        return Task.FromResult((validLines, invalidLines));
    }
}