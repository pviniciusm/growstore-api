using GrowStore.Application.Auth.DTOs;
using GrowStore.Application.Auth.Interfaces;
using GrowStore.Domain.Shared.Enums;
using Microsoft.AspNetCore.Mvc;

namespace GrowStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly ITokenService _tokenService;

        public AccountController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [HttpPost("test-token")]
        public async Task<IActionResult> TestToken()
        {
            var dto = new TokenRequestDto
            {
                AccountId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Email = "teste@growstore.com",
                Role = UserRole.CUSTOMER
            };

            var token = await _tokenService.GenerateToken(dto);
            return Ok(new { token });
        }
    }
}
