namespace Shopping.Application.DTOs.AccountDTOs.Responses
{
    public class LoginResponseDTO
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAtUtc { get; set; }
        public string Email { get; set; } = string.Empty;
    }
}