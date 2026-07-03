using GrowStore.API.Extensions;
using GrowStore.Application.Carts.DTOs;
using GrowStore.Application.Carts.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GrowStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        private Guid GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                throw new UnauthorizedAccessException("User ID not found in token.");
            }
            return userId;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ResponseCartDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ResponseCartDto>> GetCart()
        {
            var userId = GetUserId();
            var result = await _cartService.GetCartAsync(userId);
            return result.ToActionResult(this);
        }

        [HttpPost("items")]
        [ProducesResponseType(typeof(ResponseCartDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ResponseCartDto>> AddItem([FromBody] CreateCartItemDto dto)
        {
            var userId = GetUserId();
            var result = await _cartService.AddItemAsync(userId, dto);
            return result.ToActionResult(this);
        }

        [HttpPut("items")]
        [ProducesResponseType(typeof(ResponseCartDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ResponseCartDto>> UpdateItem([FromBody] UpdateCartItemDto dto)
        {
            var userId = GetUserId();
            var result = await _cartService.UpdateItemAsync(userId, dto);
            return result.ToActionResult(this);
        }

        [HttpDelete("items/{productVariantId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> RemoveItem(Guid productVariantId)
        {
            var userId = GetUserId();

            var result = await _cartService.RemoveItemAsync(userId, productVariantId);

            return result.ToActionResult(this);
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> ClearCart()
        {
            var userId = GetUserId();
            var result = await _cartService.ClearCartAsync(userId);
            return result.ToActionResult(this);
        }
    }
}
