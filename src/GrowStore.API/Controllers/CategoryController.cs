using GrowStore.API.Extensions;
using GrowStore.Application.Categories.DTOs;
using GrowStore.Application.Categories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GrowStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ResponseCategoryDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ResponseCategoryDto>>> GetAll()
        {
            var result = await _categoryService.GetAllAsync();
            return result.ToActionResult(this);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ResponseCategoryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResponseCategoryDto>> GetById(Guid id)
        {
            var result = await _categoryService.GetByIdAsync(id);
            return result.ToActionResult(this);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        [ProducesResponseType(typeof(ResponseCategoryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ResponseCategoryDto>> Create(CreateCategoryDto dto)
        {
            var result = await _categoryService.CreateAsync(dto);
            return result.ToActionResult(this);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ResponseCategoryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResponseCategoryDto>> Update(Guid id, UpdateCategoryDto dto)
        {
            var result = await _categoryService.UpdateAsync(id, dto);
            return result.ToActionResult(this);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(Guid id)
        {
            var result = await _categoryService.DeleteAsync(id);
            return result.ToActionResult(this);
        }
    }
}