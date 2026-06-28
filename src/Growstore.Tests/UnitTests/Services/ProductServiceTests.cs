using FluentAssertions;
using GrowStore.Application.Common.Errors;
using GrowStore.Application.Common.Results;
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
    private readonly ProductService _productService;

    public ProductServiceTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _productService = new ProductService(_productRepositoryMock.Object);
    }

    [Fact]
    public async Task CreateAsync_NameAlreadyExists_ReturnsConflict()
    {
        var dto = new CreateProductDto
        {
            Name = "Prod",
            CategoryId = Guid.NewGuid()
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
            CategoryId = Guid.NewGuid()
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
            Variants = new System.Collections.Generic.List<CreateProductVariantDto> { new CreateProductVariantDto { Sku = "SKU123", Price = 1, Stock = 1 } }
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
            Variants = new System.Collections.Generic.List<CreateProductVariantDto> { new CreateProductVariantDto { Sku = "SKU123", Price = 1, Stock = 1 } }
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
}
