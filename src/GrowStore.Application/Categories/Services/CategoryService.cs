using GrowStore.Application.Categories.DTOs;
using GrowStore.Application.Categories.Interfaces;
using GrowStore.Application.Common.Errors;
using GrowStore.Application.Common.Results;
using GrowStore.Domain.Entities.Categories;
using GrowStore.Domain.Interfaces;

namespace GrowStore.Application.Categories.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<Result<ResponseCategoryDto>> CreateAsync(CreateCategoryDto dto)
    {
        if (await _categoryRepository.NameExistsAsync(dto.Name))
        {
            return Result<ResponseCategoryDto>.Failure(
                Error.Conflict("Category.NameAlreadyExists", "A category with this name already exists."));
        }

        var category = Category.Create(dto.Name, dto.Description);
        await _categoryRepository.AddAsync(category);

        return Result<ResponseCategoryDto>.Success(MapToResponse(category));
    }

    public async Task<Result<ResponseCategoryDto>> GetByIdAsync(Guid id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);

        if (category is null)
        {
            return Result<ResponseCategoryDto>.Failure(
                Error.NotFound("Category.NotFound", "Category not found."));
        }

        return Result<ResponseCategoryDto>.Success(MapToResponse(category));
    }

    public async Task<Result<IEnumerable<ResponseCategoryDto>>> GetAllAsync()
    {
        var categories = await _categoryRepository.GetAllAsync();
        var response = categories.Select(MapToResponse);

        return Result<IEnumerable<ResponseCategoryDto>>.Success(response);
    }

    public async Task<Result<ResponseCategoryDto>> UpdateAsync(Guid id, UpdateCategoryDto dto)
    {
        var category = await _categoryRepository.GetByIdAsync(id);

        if (category is null)
        {
            return Result<ResponseCategoryDto>.Failure(
                Error.NotFound("Category.NotFound", "Category not found."));
        }

        category.Update(dto.Name, dto.Description);
        await _categoryRepository.UpdateAsync(category);

        return Result<ResponseCategoryDto>.Success(MapToResponse(category));
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);

        if (category is null)
        {
            return Result.Failure(
                Error.NotFound("Category.NotFound", "Category not found."));
        }

        await _categoryRepository.DeleteAsync(id);

        return Result.Success();
    }

    private static ResponseCategoryDto MapToResponse(Category category) => new()
    {
        Id = category.Id,
        Name = category.Name,
        Description = category.Description,
        CreatedAt = category.CreatedAt,
        UpdatedAt = category.UpdatedAt
    };
}