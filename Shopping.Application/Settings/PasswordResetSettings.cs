namespace Shopping.Application.Settings
{
    public sealed class PasswordResetSettings
    {
        public string ResetPageUrl { get; set; } = string.Empty;
        public int TokenExpiryMinutes { get; set; } = 15;
        public int RequestWaitingSeconds { get; set; } = 30;
    }
}
