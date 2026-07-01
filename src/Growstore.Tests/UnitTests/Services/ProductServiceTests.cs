using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using GrowStore.Application.Products.DTOs;
using GrowStore.Application.Products.Services;
using GrowStore.Domain.Entities.Categories;
using GrowStore.Domain.Entities.Products;
using GrowStore.Domain.Interfaces;
using Moq;

namespace Growstore.Tests.UnitTests.Services;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<IValidator<CreateProductDto>> _createProductValidatorMock;
    private readonly Mock<IValidator<UpdateProductDto>> _updateProductValidatorMock;
    private readonly ProductService _productService;

    public ProductServiceTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _createProductValidatorMock = new Mock<IValidator<CreateProductDto>>();
        _updateProductValidatorMock = new Mock<IValidator<UpdateProductDto>>();

        _createProductValidatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<CreateProductDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _updateProductValidatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<UpdateProductDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _productService = new ProductService(
            _productRepositoryMock.Object,
            _createProductValidatorMock.Object,
            _updateProductValidatorMock.Object);
    }

    [Fact]
    public async Task CreateAsync_WithInvalidDto_ReturnsValidationError()
    {
        // Arrange
        var dto = new CreateProductDto();

        var validationErrors = new List<ValidationFailure>
        {
            new("Name", "Product name is required."),
            new("Price", "Price must be greater than 0.")
        };

        _createProductValidatorMock
            .Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(validationErrors));

        // Act
        var result = await _productService.CreateAsync(dto);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("Product.Validation");
        result.Error.Message.Should().Contain("Product name is required.");
    }

    [Fact]
    public async Task CreateAsync_NameAlreadyExists_ReturnsConflict()
    {
        var dto = new CreateProductDto
        {
            Name = "Prod",
            CategoryId = Guid.NewGuid(),
            Variants = new List<CreateProductVariantDto> { new() }
        };

        _productRepositoryMock.Setup(r => r.NameExistsAsync(dto.Name)).ReturnsAsync(true);

        var result = await _productService.CreateAsync(dto);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("Product.NameAlreadyExists");
    }

    [Fact]
    public async Task CreateAsync_CategoryNotFound_ReturnsNotFound()
    {
        var dto = new CreateProductDto
        {
            Name = "Prod",
            CategoryId = Guid.NewGuid(),
            Variants = new List<CreateProductVariantDto> { new() }
        };

        _productRepositoryMock.Setup(r => r.NameExistsAsync(dto.Name)).ReturnsAsync(false);
        _productRepositoryMock.Setup(r => r.CategoryExistsAsync(dto.CategoryId)).ReturnsAsync(false);

        var result = await _productService.CreateAsync(dto);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("Category.NotFound");
    }

    [Fact]
    public async Task CreateAsync_SkuAlreadyExists_ReturnsConflict()
    {
        var catId = Guid.NewGuid();
        var dto = new CreateProductDto
        {
            Name = "Prod",
            CategoryId = catId,
            Variants = new List<CreateProductVariantDto> {
                new CreateProductVariantDto { Sku = "SKU123", Price = 1, Stock = 1 }
            }
        };

        _productRepositoryMock.Setup(r => r.NameExistsAsync(dto.Name)).ReturnsAsync(false);
        _productRepositoryMock.Setup(r => r.CategoryExistsAsync(dto.CategoryId)).ReturnsAsync(true);
        _productRepositoryMock.Setup(r => r.SkuExistsAsync("SKU123")).ReturnsAsync(true);

        var result = await _productService.CreateAsync(dto);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("ProductVariant.SkuAlreadyExists");
    }

    [Fact]
    public async Task CreateAsync_ValidData_ReturnsProductWithCategoryName()
    {
        var catId = Guid.NewGuid();
        var category = Category.Create("Cat", null);
        var dto = new CreateProductDto
        {
            Name = "Prod",
            CategoryId = catId,
            Variants = new List<CreateProductVariantDto> {
                new CreateProductVariantDto { Sku = "SKU123", Price = 1, Stock = 1 }
            }
        };

        _productRepositoryMock.Setup(r => r.NameExistsAsync(dto.Name)).ReturnsAsync(false);
        _productRepositoryMock.Setup(r => r.CategoryExistsAsync(dto.CategoryId)).ReturnsAsync(true);
        _productRepositoryMock.Setup(r => r.SkuExistsAsync(It.IsAny<string?>())).ReturnsAsync(false);

        Product? capturedProduct = null;
        _productRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Product>()))
            .Callback<Product>(p => capturedProduct = p)
            .Returns(Task.CompletedTask);

        _productRepositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Guid id) =>
            {
                if (capturedProduct is null) return null;

                var prod = capturedProduct;
                prod.GetType().GetProperty("Category")?.SetValue(prod, category);
                return prod;
            });

        var result = await _productService.CreateAsync(dto);

        result.IsFailure.Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value!.CategoryName.Should().Be("Cat");
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidDto_ReturnsValidationError()
    {
        // Arrange
        var id = Guid.NewGuid();
        var dto = new UpdateProductDto();

        var validationErrors = new List<ValidationFailure>
        {
            new("Name", "Product name is required."),
            new("Price", "Price must be greater than 0.")
        };

        _updateProductValidatorMock
            .Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(validationErrors));

        // Act
        var result = await _productService.UpdateAsync(id, dto);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("Product.Validation");
    }

    [Fact]
    public async Task UpdateAsync_ProductNotFound_ReturnsNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var dto = new UpdateProductDto
        {
            Name = "Updated Product",
            Price = 100
        };

        _productRepositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Product?)null);

        // Act
        var result = await _productService.UpdateAsync(id, dto);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("Product.NotFound");
    }

    [Fact]
    public async Task UpdateAsync_ValidData_ReturnsSuccess()
    {
        // Arrange
        var id = Guid.NewGuid();
        var dto = new UpdateProductDto
        {
            Name = "Updated Product",
            Price = 100
        };

        var product = Product.Create("Old Product", null, 50, null, Guid.NewGuid());

        _productRepositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(product);
        _productRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Product>())).Returns(Task.CompletedTask);
        _productRepositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(product);

        // Act
        var result = await _productService.UpdateAsync(id, dto);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Name.Should().Be("Updated Product");
    }

    [Fact]
    public async Task GetAllPagedAsync_MinPriceGreaterThanMaxPrice_ReturnsValidationError()
    {
        var filter = new ProductFilterDto
        {
            MinPrice = 100,
            MaxPrice = 10
        };

        var result = await _productService.GetAllPagedAsync(filter);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("Product.InvalidPriceRange");
    }

    [Fact]
    public async Task GetAllPagedAsync_CategoryNotFound_ReturnsNotFound()
    {
        var categoryId = Guid.NewGuid();
        var filter = new ProductFilterDto
        {
            CategoryId = categoryId
        };

        _productRepositoryMock.Setup(r => r.CategoryExistsAsync(categoryId)).ReturnsAsync(false);

        var result = await _productService.GetAllPagedAsync(filter);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("Category.NotFound");
    }

    [Fact]
    public async Task GetAllPagedAsync_ValidFilter_ReturnsPagedResultWithMappedItems()
    {
        var categoryId = Guid.NewGuid();
        var category = Category.Create("Plantas", null);
        var product = Product.Create("Vaso Cerâmico", "Descrição", 29.90m, null, categoryId);
        product.GetType().GetProperty("Category")?.SetValue(product, category);

        var filter = new ProductFilterDto
        {
            Name = "Vaso",
            CategoryId = categoryId,
            MinPrice = 10,
            MaxPrice = 50,
            Page = 1,
            Limit = 10
        };

        _productRepositoryMock.Setup(r => r.CategoryExistsAsync(categoryId)).ReturnsAsync(true);
        _productRepositoryMock
            .Setup(r => r.GetAllPagedAsync("Vaso", categoryId, 10, 50, 1, 10))
            .ReturnsAsync((new List<Product> { product }, 1));

        var result = await _productService.GetAllPagedAsync(filter);

        result.IsFailure.Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value!.TotalCount.Should().Be(1);
        result.Value!.Page.Should().Be(1);
        result.Value!.Limit.Should().Be(10);
        result.Value!.Items.Should().ContainSingle();
        result.Value!.Items.First().Name.Should().Be("Vaso Cerâmico");
        result.Value!.Items.First().CategoryName.Should().Be("Plantas");
    }

    [Fact]
    public async Task GetAllPagedAsync_PageLessThanOne_ClampsToPageOne()
    {
        var filter = new ProductFilterDto
        {
            Page = 0,
            Limit = 10
        };

        _productRepositoryMock
            .Setup(r => r.GetAllPagedAsync(null, null, null, null, 1, 10))
            .ReturnsAsync((new List<Product>(), 0));

        var result = await _productService.GetAllPagedAsync(filter);

        result.IsFailure.Should().BeFalse();
        result.Value!.Page.Should().Be(1);
        _productRepositoryMock.Verify(r => r.GetAllPagedAsync(null, null, null, null, 1, 10), Times.Once);
    }

    [Fact]
    public async Task GetAllPagedAsync_LimitOutOfRange_ClampsToDefaultTen()
    {
        var filter = new ProductFilterDto
        {
            Page = 1,
            Limit = 500
        };

        _productRepositoryMock
            .Setup(r => r.GetAllPagedAsync(null, null, null, null, 1, 10))
            .ReturnsAsync((new List<Product>(), 0));

        var result = await _productService.GetAllPagedAsync(filter);

        result.IsFailure.Should().BeFalse();
        result.Value!.Limit.Should().Be(10);
        _productRepositoryMock.Verify(r => r.GetAllPagedAsync(null, null, null, null, 1, 10), Times.Once);
    }

    [Fact]
    public async Task GetAllPagedAsync_NoFilters_ReturnsEmptyPagedResultWhenNoProducts()
    {
        var filter = new ProductFilterDto();

        _productRepositoryMock
            .Setup(r => r.GetAllPagedAsync(null, null, null, null, 1, 10))
            .ReturnsAsync((new List<Product>(), 0));

        var result = await _productService.GetAllPagedAsync(filter);

        result.IsFailure.Should().BeFalse();
        result.Value!.Items.Should().BeEmpty();
        result.Value!.TotalCount.Should().Be(0);
    }
}
