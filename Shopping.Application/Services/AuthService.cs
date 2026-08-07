using FluentValidation;
using Microsoft.Extensions.Logging;
using Shopping.Application.DTOs.AccountDTOs.Requests;
using Shopping.Application.DTOs.AccountDTOs.Responses;
using Shopping.Application.Handlers.Exceptions;
using Shopping.Application.ServiceInterfaces;
using Shopping.Domain.Entities.Accounts;
using Shopping.Domain.Interfaces;
using System.Security.Cryptography;
using ValidationException = Shopping.Application.Handlers.Exceptions.ValidationException;

namespace Shopping.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailVerificationRepository _emailVerificationRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IEmailService _emailService;
        private readonly IValidator<RegisterRequestDTO> _registerValidator;
        private readonly ILogger<AuthService> _logger;
        private const int MaxAttempts = 5;
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        public AuthService(
            IUserRepository userRepository,
            IEmailVerificationRepository emailVerificationRepository,
            IPasswordHasher passwordHasher,
            IEmailService emailService,
            IValidator<RegisterRequestDTO> registerValidator,
            ITokenService tokenService,
            ILogger<AuthService> logger,
            IRefreshTokenRepository refreshTokenRepository)
        {
            _userRepository = userRepository;
            _emailVerificationRepository = emailVerificationRepository;
            _passwordHasher = passwordHasher;
            _emailService = emailService;
            _registerValidator = registerValidator;
            _logger = logger;
            _tokenService = tokenService;
            _refreshTokenRepository = refreshTokenRepository;
        }
        public async Task LogoutAsync(string? refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                throw new UnauthorizedException("Refresh token is missing.");
            }

            var tokenHash =
                _tokenService.HashRefreshToken(refreshToken);

            var storedToken =
                await _refreshTokenRepository
                    .GetByTokenHashAsync(tokenHash);

            if (storedToken is null ||
                storedToken.RevokedAtUtc is not null)
            {
                return;
            }

            storedToken.RevokedAtUtc = DateTime.UtcNow;

            await _refreshTokenRepository
                .RevokeAsync(storedToken);
        }
        public async Task<AuthenticationResult> LoginAsync(LoginRequestDTO dto)
        {
            var email = dto.Email.Trim().ToLowerInvariant();

            var user = await _userRepository.GetByEmailAsync(email);

            if (user is null ||
                !_passwordHasher.Verify(
                    dto.Password,
                    user.PasswordHash))
            {
                throw new UnauthorizedException(
                    "Email or password is incorrect.");
            }

            if (!user.EmailConfirmed)
            {
                throw new ConflictException(
                    "Email is not confirmed.");
            }

            var accessToken = _tokenService.CreateAccessToken(user);

            var refreshToken = _tokenService.CreateRefreshToken();

            var refreshTokenEntity = new RefreshToken
            {
                UserId = user.Id,
                TokenHash = refreshToken.TokenHash,
                ExpiresAtUtc = refreshToken.ExpiresAtUtc
            };

            await _refreshTokenRepository.AddAsync(refreshTokenEntity);

            return CreateAuthenticationResult(user, accessToken, refreshToken);
        }
        public async Task<AuthenticationResult> RefreshAsync(string? refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                throw new UnauthorizedException("Refresh token is invalid.");
            }

            var tokenHash = _tokenService.HashRefreshToken(refreshToken);
            var currentToken = await _refreshTokenRepository.GetByTokenHashAsync(tokenHash);

            if (currentToken is null)
            {
                throw new UnauthorizedException("Refresh token is invalid.");
            }

            if (currentToken.RevokedAtUtc is not null)
            {
                await _refreshTokenRepository.RevokeAllActiveForUserAsync(currentToken.UserId);
                throw new UnauthorizedException("Refresh token is invalid.");
            }

            var utcNow = DateTime.UtcNow;

            if (currentToken.ExpiresAtUtc <= utcNow)
            {
                throw new UnauthorizedException("Refresh token has expired.");
            }

            var user = currentToken.User;

            if (!user.EmailConfirmed)
            {
                throw new UnauthorizedException("User cannot refresh this session.");
            }

            var newAccessToken = _tokenService.CreateAccessToken(user);
            var newRefreshToken = _tokenService.CreateRefreshToken();

            currentToken.RevokedAtUtc = utcNow;

            var newRefreshTokenEntity = new RefreshToken
            {
                UserId = user.Id,
                TokenHash = newRefreshToken.TokenHash,
                ExpiresAtUtc = newRefreshToken.ExpiresAtUtc
            };

            await _refreshTokenRepository.RotateAsync(currentToken, newRefreshTokenEntity);

            return CreateAuthenticationResult(user, newAccessToken, newRefreshToken);
        }
        public async Task VerifyEmailAsync(VerifyEmailRequestDTO dto)
        {
            var email = dto.Email.Trim().ToLowerInvariant();
            var user = await _userRepository.GetByEmailAsync(email);

            if (user is null)
            {
                throw new NotFoundException("User is not found.");
            }

            if (user.EmailConfirmed)
            {
                throw new ConflictException("Email is already confirmed.");
            }

            var verification = await _emailVerificationRepository.GetLatestActiveAsync(user.Id);

            if (verification is null)
            {
                throw new ValidationException("Code is expired or does not exist. Please request a new one.");
            }

            if (verification.AttemptCount >= MaxAttempts)
            {
                throw new ValidationException("Too many wrong attempts. Please request a new code.");
            }

            if (!_passwordHasher.Verify(dto.Code.Trim(), verification.CodeHash))
            {
                verification.AttemptCount++;
                await _emailVerificationRepository.UpdateAsync(verification);

                throw new ValidationException("Code is not correct.");
            }

            verification.UsedAt = DateTime.UtcNow;
            await _emailVerificationRepository.UpdateAsync(verification);

            user.EmailConfirmed = true;
            await _userRepository.UpdateAsync(user);
        }

        public async Task<UserResponseDTO> RegisterAsync(RegisterRequestDTO dto)
        {
            var validationResult = await _registerValidator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                var message = string.Join(" ", validationResult.Errors.Select(e => e.ErrorMessage));
                throw new ValidationException(message);
            }

            var email = dto.Email.Trim().ToLowerInvariant();

            if (await _userRepository.EmailExistsAsync(email))
            {
                throw new ConflictException("This email is already registered.");
            }

            var user = new User
            {
                Name = dto.Name.Trim(),
                Surname = dto.Surname.Trim(),
                Email = email,
                PasswordHash = _passwordHasher.Hash(dto.Password),
                EmailConfirmed = false
            };

            await _userRepository.AddAsync(user);
            var code = GenerateCode();

            var verification = new EmailVerification
            {
                UserId = user.Id,
                CodeHash = _passwordHasher.Hash(code),
                ExpiresAt = DateTime.UtcNow.AddMinutes(15),
                AttemptCount = 0
            };

            await _emailVerificationRepository.AddAsync(verification);

            try
            {
                await _emailService.SendAsync(
                    user.Email,
                    "Verify your email",
                    $"<p>Your verification code is <b>{code}</b></p><p>It expires in 15 minutes.</p>");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not send verification email to {Email}", user.Email);
            }

            return new UserResponseDTO
            {
                Id = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed
            };
        }
        private static string GenerateCode()
        {
            return RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");
        }
        private static AuthenticationResult CreateAuthenticationResult(User user, TokenResult accessToken, RefreshTokenResult refreshToken)
        {
            return new AuthenticationResult
            {
                AccessToken = accessToken.Token,
                AccessTokenExpiresAtUtc = accessToken.ExpiresAtUtc,
                RefreshToken = refreshToken.Token,
                RefreshTokenExpiresAtUtc = refreshToken.ExpiresAtUtc,
                Email = user.Email
            };
        }
    }
}