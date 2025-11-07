using CNAB.Application.DTOs.Account;

namespace CNAB.Application.Interfaces.Account;

public interface ITokenService
{
    UserTokenDto GenerateToken(string email);
}