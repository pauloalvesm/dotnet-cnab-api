using CNAB.Application.DTOs.Account;
using CNAB.Domain.Entities.Account;
using Mapster;

namespace CNAB.Application.Mappings;

public class UserMapping : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<User, UserDto>();
        config.NewConfig<UserDto, User>();

        config.NewConfig<Login, LoginDto>();
        config.NewConfig<LoginDto, Login>();

        config.NewConfig<UserToken, UserTokenDto>();
        config.NewConfig<UserTokenDto, UserToken>();
    }
}