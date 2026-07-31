using FluentValidation;
using Shopping.Application.DTOs.AccountDTOs.Requests;
using Shopping.Application.DTOs.AccountDTOs.Responses;
using Shopping.Application.Handlers.Exceptions;
using Shopping.Application.ServiceInterfaces;
using Shopping.Domain.Entities.Accounts;
using Shopping.Domain.Interfaces;
using ValidationException = Shopping.Application.Handlers.Exceptions.ValidationException;

namespace Shopping.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IValidator<RegisterRequestDTO> _registerValidator;

        public AuthService(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IValidator<RegisterRequestDTO> registerValidator)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _registerValidator = registerValidator;
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

            return new UserResponseDTO
            {
                Id = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed
            };
        }
    }
}