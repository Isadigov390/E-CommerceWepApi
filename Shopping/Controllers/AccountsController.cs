using Microsoft.AspNetCore.Mvc;
using Shopping.Application.DTOs.AccountDTOs.Requests;
using Shopping.Application.ServiceInterfaces;

namespace Shopping.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AccountsController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO registerDTO)
        {
            var user = await _authService.RegisterAsync(registerDTO);
            return CreatedAtAction(nameof(Register), new { id = user.Id }, user);
        }
    }
}