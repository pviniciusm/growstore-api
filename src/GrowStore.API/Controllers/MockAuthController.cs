using GrowStore.Application.Auth.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GrowStore.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MockAuthController : ControllerBase
{

    private readonly IAuthService _authService;

    public MockAuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("mock-login")]
    public IActionResult MockLogin()
    {
        return Ok(new
        {
            Token = _authService.GenerateTestToken()
        });
    }
}
