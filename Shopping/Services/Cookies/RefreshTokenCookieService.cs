namespace Shopping.WebApi.Services.Cookies
{
    public sealed class RefreshTokenCookieService
    {
        private const string CookieName = "refreshToken";
        private const string CookiePath = "/api/accounts";

        public string? Read(HttpRequest request)
        {
            return request.Cookies[CookieName];
        }

        public void Write(HttpResponse response, string refreshToken, DateTime expiresAtUtc)
        {
            var cookieOptions = CreateCookieOptions();
            cookieOptions.Expires = expiresAtUtc;

            response.Cookies.Append(CookieName, refreshToken, cookieOptions);
        }

        public void Delete(HttpResponse response)
        {
            var cookieOptions = CreateCookieOptions();

            response.Cookies.Delete(CookieName, cookieOptions);
        }

        private static CookieOptions CreateCookieOptions()
        {
            return new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Path = CookiePath
            };
        }
    }
}