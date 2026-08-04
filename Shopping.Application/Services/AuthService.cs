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
        public AuthService(
            IUserRepository userRepository,
            IEmailVerificationRepository emailVerificationRepository,
            IPasswordHasher passwordHasher,
            IEmailService emailService,
            IValidator<RegisterRequestDTO> registerValidator,
            ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _emailVerificationRepository = emailVerificationRepository;
            _passwordHasher = passwordHasher;
            _emailService = emailService;
            _registerValidator = registerValidator;
            _logger = logger;
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
    }
}