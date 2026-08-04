using Shopping.Application.DTOs.AccountDTOs.Requests;
using Shopping.Application.DTOs.AccountDTOs.Responses;

namespace Shopping.Application.ServiceInterfaces
{
    public interface IAuthService
    {
        Task VerifyEmailAsync(VerifyEmailRequestDTO dto);
        Task<UserResponseDTO> RegisterAsync(RegisterRequestDTO dto);
    }
}