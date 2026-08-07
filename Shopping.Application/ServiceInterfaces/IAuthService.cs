using Shopping.Application.DTOs.AccountDTOs.Requests;
using Shopping.Application.DTOs.AccountDTOs.Responses;

namespace Shopping.Application.ServiceInterfaces
{
    public interface IAuthService
    {
        Task<UserResponseDTO> RegisterAsync(RegisterRequestDTO dto);
        Task VerifyEmailAsync(VerifyEmailRequestDTO dto);
        Task<AuthenticationResult> LoginAsync(LoginRequestDTO dto);
        Task<AuthenticationResult> RefreshAsync(string? refreshToken);
        Task LogoutAsync(string? refreshToken); 
    }
}
