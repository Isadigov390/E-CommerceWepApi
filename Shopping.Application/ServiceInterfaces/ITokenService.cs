using Shopping.Application.DTOs.AccountDTOs.Responses;
using Shopping.Domain.Entities.Accounts;

namespace Shopping.Application.ServiceInterfaces
{
    public interface ITokenService
    {
        TokenResult CreateToken(User user);
    }
}