using GrowStore.Application.Categories.DTOs;
using GrowStore.Application.Common.Results;

namespace GrowStore.Application.Categories.Interfaces;

public interface ICategoryService
{
    Task<Result<ResponseCategoryDto>> CreateAsync(CreateCategoryDto dto);
    Task<Result<ResponseCategoryDto>> GetByIdAsync(Guid id);
    Task<Result<IEnumerable<ResponseCategoryDto>>> GetAllAsync();
    Task<Result<ResponseCategoryDto>> UpdateAsync(Guid id, UpdateCategoryDto dto);
    Task<Result> DeleteAsync(Guid id);
}