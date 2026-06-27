using GrowStore.API.Extensions;
using GrowStore.Application.Users.DTOs;
using GrowStore.Application.Users.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GrowStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResponseUserDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseUserDto>> Create([FromBody] CreateUserDto dto)
        {
            var result = await _userService.CreateUserAsync(dto);
            return result.ToActionResult(this);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ResponseUserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResponseUserDto>> GetById(Guid id)
        {
            var result = await _userService.GetUserByIdAsync(id);
            return result.ToActionResult(this);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Update(Guid id, [FromBody] UpdateUserDto dto)
        {
            var result = await _userService.UpdateUserAsync(id, dto);
            return result.ToActionResult(this);
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(Guid id)
        {
            var result = await _userService.DeleteUserAsync(id);
            return result.ToActionResult(this);
        }
    }
}