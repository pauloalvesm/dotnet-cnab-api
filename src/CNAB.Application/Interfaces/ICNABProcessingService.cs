using CNAB.Application.DTOs;
using CNAB.Domain.Entities;

namespace CNAB.Application.Interfaces;

public interface ICNABProcessingService
{
    Task<ParseResultDto> ParseCNABAsync(IEnumerable<string> lines);
    Task<int> ProcessValidLinesAsync(IEnumerable<string> validLines);
    Task<Transaction> ParseLineAsync(string line);
    Task<Store> GetOrCreateStoreAsync(string storeName, string owner);
    Task SaveTransactionAsync(Transaction transaction);
    Task<(List<string> validLines, List<string> invalidLines)> ValidateLinesAsync(IEnumerable<string> lines);
}