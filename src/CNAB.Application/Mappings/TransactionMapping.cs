using CNAB.Application.DTOs;
using CNAB.Domain.Entities;
using CNAB.Domain.Entities.enums;
using Mapster;

namespace CNAB.Application.Mappings;

public class TransactionMapping : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Transaction, TransactionDto>()
            .Map(dest => dest.Type, src => (int)src.Type)
            .Map(dest => dest.StoreId, src => src.Store.Id)
            .Map(dest => dest.StoreName, src => src.Store.Name)
            .Map(dest => dest.StoreOwnerName, src => src.Store.OwnerName);

        config.NewConfig<TransactionDto, Transaction>()
            .Map(dest => dest.Type, src => (TransactionType)src.Type)
            .Ignore(dest => dest.Store)
            .Ignore(dest => dest.SignedAmount)
            .Ignore(dest => dest.IsExpense)
            .Ignore(dest => dest.IsIncome);
    }
}