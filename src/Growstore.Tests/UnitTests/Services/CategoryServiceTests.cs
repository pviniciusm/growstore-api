using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using GrowStore.Application.Categories.DTOs;
using GrowStore.Application.Categories.Services;
using GrowStore.Application.Common.Errors;
using GrowStore.Domain.Entities.Categories;
using GrowStore.Domain.Interfaces;
using Moq;

namespace Growstore.Tests.UnitTests.Services;

public class CategoryServiceTests
{
    private readonly Mock<ICategoryRepository> _categoryRepoMock;
    private readonly Mock<IValidator<CreateCategoryDto>> _createValidatorMock;
    private readonly Mock<IValidator<UpdateCategoryDto>> _updateValidatorMock;
    private readonly CategoryService _categoryService;

    public CategoryServiceTests()
    {
        _categoryRepoMock = new Mock<ICategoryRepository>();
        _createValidatorMock = new Mock<IValidator<CreateCategoryDto>>();
        _updateValidatorMock = new Mock<IValidator<UpdateCategoryDto>>();

        _createValidatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<CreateCategoryDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _updateValidatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<UpdateCategoryDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _categoryService = new CategoryService(
            _categoryRepoMock.Object,
            _createValidatorMock.Object,
            _updateValidatorMock.Object);
    }

    [Fact]
    public async Task CreateAsync_WithInvalidDto_ReturnsValidationError()
    {
        var dto = new CreateCategoryDto();
        var errors = new List<ValidationFailure> { new("Name", "Name is required.") };

        _createValidatorMock
            .Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(errors));

        var result = await _categoryService.CreateAsync(dto);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("Category.Validation");
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateName_ReturnsConflict()
    {
        var dto = new CreateCategoryDto { Name = "Existing", Description = "Desc" };

        _categoryRepoMock.Setup(r => r.NameExistsAsync(dto.Name)).ReturnsAsync(true);

        var result = await _categoryService.CreateAsync(dto);

        result.IsFailure.Should().BeTrue();
        result.Error!.Type.Should().Be(ErrorType.Conflict);
    }

    [Fact]
    public async Task CreateAsync_WithValidData_ReturnsSuccess()
    {
        var dto = new CreateCategoryDto { Name = "Electronics", Description = "Desc" };

        _categoryRepoMock.Setup(r => r.NameExistsAsync(dto.Name)).ReturnsAsync(false);

        var result = await _categoryService.CreateAsync(dto);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Name.Should().Be(dto.Name);
        _categoryRepoMock.Verify(r => r.AddAsync(It.IsAny<Category>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_NonExisting_ReturnsNotFound()
    {
        var id = Guid.NewGuid();
        _categoryRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Category?)null);

        var result = await _categoryService.GetByIdAsync(id);

        result.IsFailure.Should().BeTrue();
        result.Error!.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task GetByIdAsync_Existing_ReturnsSuccess()
    {
        var category = Category.Create("Electronics", "Desc");
        _categoryRepoMock.Setup(r => r.GetByIdAsync(category.Id)).ReturnsAsync(category);

        var result = await _categoryService.GetByIdAsync(category.Id);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Name.Should().Be("Electronics");
    }

    [Fact]
    public async Task GetAllAsync_ReturnsSuccess()
    {
        var categories = new List<Category>
        {
            Category.Create("Cat1", null),
            Category.Create("Cat2", "Desc")
        };

        _categoryRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(categories);

        var result = await _categoryService.GetAllAsync();

        result.IsSuccess.Should().BeTrue();
        result.Value!.Count().Should().Be(2);
    }

    [Fact]
    public async Task UpdateAsync_NonExisting_ReturnsNotFound()
    {
        var id = Guid.NewGuid();
        var dto = new UpdateCategoryDto { Name = "Updated" };

        _categoryRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Category?)null);

        var result = await _categoryService.UpdateAsync(id, dto);

        result.IsFailure.Should().BeTrue();
        result.Error!.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task UpdateAsync_Existing_ReturnsSuccess()
    {
        var category = Category.Create("Old", "Desc");
        var dto = new UpdateCategoryDto { Name = "New", Description = "New Desc" };

        _categoryRepoMock.Setup(r => r.GetByIdAsync(category.Id)).ReturnsAsync(category);

        var result = await _categoryService.UpdateAsync(category.Id, dto);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Name.Should().Be("New");
        _categoryRepoMock.Verify(r => r.UpdateAsync(category), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_NonExisting_ReturnsNotFound()
    {
        var id = Guid.NewGuid();
        _categoryRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Category?)null);

        var result = await _categoryService.DeleteAsync(id);

        result.IsFailure.Should().BeTrue();
        result.Error!.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task DeleteAsync_Existing_ReturnsSuccess()
    {
        var category = Category.Create("ToDelete", null);
        _categoryRepoMock.Setup(r => r.GetByIdAsync(category.Id)).ReturnsAsync(category);

        var result = await _categoryService.DeleteAsync(category.Id);

        result.IsSuccess.Should().BeTrue();
        _categoryRepoMock.Verify(r => r.DeleteAsync(category.Id), Times.Once);
    }
}
