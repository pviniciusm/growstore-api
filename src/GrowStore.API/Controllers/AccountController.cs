using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GrowStore.Application.Accounts.Interfaces;
using GrowStore.Application.Accounts.DTOs;

namespace GrowStore.API.Controllers;

[ApiController]
[Route("api/[controller]")] 
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpPost] 
    public async Task<IActionResult> Login([FromBody] CreateAccountDto request)
    {
        var token = await _accountService.LoginAsync(request.Email, request.Password);
        return Ok(new { Token = token });
    }
}