namespace Shopping.Application.DTOs.AccountDTOs.Responses
{
    public class TokenResult
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAtUtc { get; set; }
    }
}