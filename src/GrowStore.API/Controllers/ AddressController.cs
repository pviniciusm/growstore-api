using GrowStore.API.Extensions;
using GrowStore.Application.Addresses.DTOs;
using GrowStore.Application.Addresses.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GrowStore.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AddressController : ControllerBase
    {
        private readonly IAddressService _addressService;

        public AddressController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(AddressResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AddressResponseDto>> GetById(Guid id)
        {
            var result = await _addressService.GetByIdAsync(id);
            return result.ToActionResult(this);
        }

        [HttpGet("user/{userId:guid}")]
        [ProducesResponseType(typeof(IEnumerable<AddressResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<AddressResponseDto>>> GetByUserId(Guid userId)
        {
            var result = await _addressService.GetByUserIdAsync(userId);
            return result.ToActionResult(this);
        }

        [HttpPost]
        [ProducesResponseType(typeof(AddressResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AddressResponseDto>> Create(CreateAddressDto dto)
        {
            var result = await _addressService.CreateAsync(dto);
            return result.ToActionResult(this);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(AddressResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AddressResponseDto>> Update(Guid id, UpdateAddressDto dto)
        {
            var result = await _addressService.UpdateAsync(id, dto);
            return result.ToActionResult(this);
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(Guid id)
        {
            var result = await _addressService.DeleteAsync(id);
            return result.ToActionResult(this);
        }
    }
}