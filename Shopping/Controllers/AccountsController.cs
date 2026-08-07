using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shopping.Application.DTOs.AccountDTOs.Requests;
using Shopping.Application.DTOs.AccountDTOs.Responses;
using Shopping.Application.ServiceInterfaces;

namespace Shopping.WebApi.Controllers
{
    [ApiController]
    [Route("api/accounts")]

    public class AccountsController : ControllerBase
    {
        private const string RefreshTokenCookieName = "refreshToken";
        private readonly IAuthService _authService;

        public AccountsController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO request)
        {
            var user = await _authService.RegisterAsync(request);
            return StatusCode(StatusCodes.Status201Created, user);
        }

        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequestDTO request)
        {
            await _authService.VerifyEmailAsync(request);
            return NoContent();
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDTO>> Login([FromBody] LoginRequestDTO request)
        {
            var result = await _authService.LoginAsync(request);

            WriteRefreshTokenCookie(result);

            return Ok(CreateLoginResponse(result));
        }

        [Produces("application/json")]
        [HttpPost("refresh")]
        public async Task<ActionResult<LoginResponseDTO>> Refresh()
        {
            var refreshToken = Request.Cookies[RefreshTokenCookieName];
            var result = await _authService.RefreshAsync(refreshToken);

            WriteRefreshTokenCookie(result);

            return Ok(CreateLoginResponse(result));
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies[RefreshTokenCookieName];

            if (!string.IsNullOrWhiteSpace(refreshToken))
            {
                await _authService.LogoutAsync(refreshToken);
            }

            DeleteRefreshTokenCookie();

            return NoContent();
        }

        private void WriteRefreshTokenCookie(AuthenticationResult result)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = result.RefreshTokenExpiresAtUtc,
                Path = "/api/accounts"
            };

            Response.Cookies.Append(
                RefreshTokenCookieName,
                result.RefreshToken,
                cookieOptions);
        }

        private void DeleteRefreshTokenCookie()
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Path = "/api/accounts"
            };

            Response.Cookies.Delete(
                RefreshTokenCookieName,
                cookieOptions);
        }

        private static LoginResponseDTO CreateLoginResponse(AuthenticationResult result)
        {
            return new LoginResponseDTO
            {
                AccessToken = result.AccessToken,
                ExpiresAtUtc = result.AccessTokenExpiresAtUtc,
                Email = result.Email
            };
        }
    }
}