using GrowStore.API.Extensions;
using GrowStore.Application.Auth.DTOs;
using GrowStore.Application.Auth.Interfaces;
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
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<AuthResponseDto>> Register(RegisterRequestDto request)
        {
            var result = await _authService.RegisterAsync(request);
            return result.ToActionResult(this);
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginRequestDto request)
        {
            var result = await _authService.LoginAsync(request);
            return result.ToActionResult(this);
        }

        [HttpPost("refresh")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AuthResponseDto>> Refresh(RefreshTokenRequestDto request)
        {
            var result = await _authService.RefreshAsync(request);
            return result.ToActionResult(this);
        }

        [Authorize]
        [HttpGet("me")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Me()
        {
            var accountId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            var userId = User.FindFirstValue("userId");
            var email = User.FindFirstValue(JwtRegisteredClaimNames.Email);
            var role = User.FindFirstValue(ClaimTypes.Role);

            return Ok(new { accountId, userId, email, role });
        }
    }
}