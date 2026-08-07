using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shopping.Application.DTOs.AccountDTOs.Responses;
using Shopping.Application.ServiceInterfaces;
using Shopping.Domain.Entities.Accounts;
using Shopping.Infrastructure.Settings;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Shopping.Infrastructure.Authentication
{
    public class JwtTokenService : ITokenService
    {
        private readonly JwtSettings _settings;

        public JwtTokenService(IOptions<JwtSettings> options)
        {
            _settings = options.Value;
        }

        public TokenResult CreateAccessToken(User user)
        {
            var expiresAt = DateTime.UtcNow
                .AddMinutes(_settings.ExpiryMinutes);

            var claims = new List<Claim>
            {
                new Claim(
                    JwtRegisteredClaimNames.Sub,
                    user.Id.ToString()),

                new Claim(
                    JwtRegisteredClaimNames.Email,
                    user.Email),

                new Claim(
                    JwtRegisteredClaimNames.Jti,
                    Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_settings.Key));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: credentials);

            return new TokenResult
            {
                Token = new JwtSecurityTokenHandler()
                    .WriteToken(token),

                ExpiresAtUtc = expiresAt
            };
        }

        public RefreshTokenResult CreateRefreshToken()
        {
            var rawToken = Convert.ToHexString(
                RandomNumberGenerator.GetBytes(32));

            return new RefreshTokenResult
            {
                Token = rawToken,

                TokenHash = HashRefreshToken(rawToken),

                ExpiresAtUtc = DateTime.UtcNow.AddDays(
                    _settings.RefreshTokenExpiryDays)
            };
        }

        public string HashRefreshToken(string refreshToken)
        {
            var tokenBytes = Encoding.UTF8.GetBytes(refreshToken);
            var hashBytes = SHA256.HashData(tokenBytes);

            return Convert.ToHexString(hashBytes);
        }
    }
}