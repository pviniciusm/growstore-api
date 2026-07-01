using GrowStore.API.Extensions;
using GrowStore.Application.Common.Pagination;
using GrowStore.Application.Products.DTOs;
using GrowStore.Application.Products.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GrowStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<ResponseProductDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PagedResult<ResponseProductDto>>> GetAll([FromQuery] ProductFilterDto filter)
        {
            var result = await _productService.GetAllPagedAsync(filter);
            return result.ToActionResult(this);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ResponseProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResponseProductDto>> GetById(Guid id)
        {
            var result = await _productService.GetByIdAsync(id);
            return result.ToActionResult(this);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResponseProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ResponseProductDto>> Create(CreateProductDto dto)
        {
            var result = await _productService.CreateAsync(dto);
            return result.ToActionResult(this);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ResponseProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResponseProductDto>> Update(Guid id, UpdateProductDto dto)
        {
            var result = await _productService.UpdateAsync(id, dto);
            return result.ToActionResult(this);
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(Guid id)
        {
            var result = await _productService.DeleteAsync(id);
            return result.ToActionResult(this);
        }
    }
}
