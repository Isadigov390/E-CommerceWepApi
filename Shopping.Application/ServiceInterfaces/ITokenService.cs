using Shopping.Application.DTOs.AccountDTOs.Responses;
using Shopping.Domain.Entities.Accounts;

namespace Shopping.Application.ServiceInterfaces
{
    public interface ITokenService
    {
        TokenResult CreateAccessToken(User user);

        RefreshTokenResult CreateRefreshToken();

        string HashRefreshToken(string refreshToken);
    }
}
