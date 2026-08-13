namespace Shopping.Application.DTOs.AccountDTOs.Requests
{
    public class ResetPasswordRequestDTO
    {
        public string Token { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
