using CNAB.Application.DTOs;
using CNAB.Domain.Entities;
using Mapster;

namespace CNAB.Application.Mappings;

public class StoreMapping : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Store, StoreDto>()
            .Map(dest => dest.Balance, src => src.GetBalance());

        config.NewConfig<StoreDto, Store>()
            .Ignore(dest => dest.Transactions);

        config.NewConfig<StoreInputDto, Store>()
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.Transactions);
    }
}