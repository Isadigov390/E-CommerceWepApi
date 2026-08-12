using Microsoft.AspNetCore.Mvc;
using Shopping.Application.DTOs.AccountDTOs.Requests;
using Shopping.Application.DTOs.AccountDTOs.Responses;
using Shopping.Application.ServiceInterfaces;
using Shopping.WebApi.Services.Cookies;

namespace Shopping.WebApi.Controllers
{
    [ApiController]
    [Route("api/accounts")]
    public class AccountsController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly RefreshTokenCookieService _refreshTokenCookieService;

        public AccountsController(IAuthService authService, RefreshTokenCookieService refreshTokenCookieService)
        {
            _authService = authService;
            _refreshTokenCookieService = refreshTokenCookieService;
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

            _refreshTokenCookieService.Write(Response, result.RefreshToken, result.RefreshTokenExpiresAtUtc);

            return Ok(CreateLoginResponse(result));
        }

        [Produces("application/json")]
        [HttpPost("refresh")]
        public async Task<ActionResult<LoginResponseDTO>> Refresh()
        {
            var refreshToken = _refreshTokenCookieService.Read(Request);
            var result = await _authService.RefreshAsync(refreshToken);

            _refreshTokenCookieService.Write(Response, result.RefreshToken, result.RefreshTokenExpiresAtUtc);

            return Ok(CreateLoginResponse(result));
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = _refreshTokenCookieService.Read(Request);

            if (!string.IsNullOrWhiteSpace(refreshToken))
            {
                await _authService.LogoutAsync(refreshToken);
            }

            _refreshTokenCookieService.Delete(Response);

            return NoContent();
        }
        [HttpPost("resend-verification-email")]
        public async Task<IActionResult> ResendVerificationEmail([FromBody] ResendVerificationEmailRequestDTO request)
        {
            await _authService.ResendVerificationEmailAsync(request);

            return NoContent();
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