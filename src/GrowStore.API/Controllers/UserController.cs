using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GrowStore.Application.Accounts.Interfaces;
using GrowStore.Application.Accounts.DTOs;

namespace GrowStore.API.Controllers;

[ApiController]
[Route("api/[controller]")] 
public class UserController : ControllerBase
{
    private readonly IAccountService _accountService;

    public UserController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpPost] 
    public async Task<IActionResult> CreateUser([FromBody] CreateAccountDto request)
    {
        var response = await _accountService.RegisterAsync(request);
        return Created("", response);
    }

    [HttpGet("{id}")] 
    public async Task<IActionResult> GetProfile(Guid id)
    {
        var profile = await _accountService.GetProfileAsync(id);
        return Ok(profile);
    }

    // As outras operações do CRUD (PUT, DELETE) entrarão aqui no futuro!
    
}