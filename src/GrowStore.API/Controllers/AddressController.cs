using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GrowStore.Domain.Interfaces;

namespace GrowStore.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AddressController : ControllerBase
{
    private readonly IAddressRepository _addressRepository;

    public AddressController(IAddressRepository addressRepository)
    {
        _addressRepository = addressRepository;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var address = await _addressRepository.GetByIdAsync(id);
        
        if (address == null)
        {
            return NotFound(new { message = "Endereço não encontrado." });
        }

        return Ok(address);
    }
}