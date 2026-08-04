namespace Shopping.Application.DTOs.AccountDTOs.Requests
{
    public class VerifyEmailRequestDTO
    {
        public string Email { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }
}