using GrowStore.Application.Auth.DTOs;
using GrowStore.Application.Auth.Interfaces;
using GrowStore.Domain.Shared.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace GrowStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;

        public AuthController(ITokenService tokenService)
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

        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            var accountId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            var userId = User.FindFirstValue("userId");
            var email = User.FindFirstValue(JwtRegisteredClaimNames.Email);
            var role = User.FindFirstValue(ClaimTypes.Role);

            return Ok(new { accountId, userId, email, role });
        }

        [Authorize(Roles = "CUSTOMER")]
        [HttpGet("customer-only")]
        public IActionResult CustomerOnly() => Ok(new { message = "Customer access granted" });

        [Authorize(Roles = "ADMIN")]
        [HttpGet("admin-only")]
        public IActionResult AdminOnly() => Ok(new { message = "Admin access granted" });
    }
}
