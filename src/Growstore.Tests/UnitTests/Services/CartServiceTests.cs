using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using GrowStore.Application.Carts.DTOs;
using GrowStore.Application.Carts.Services;
using GrowStore.Application.Common.Errors;
using GrowStore.Domain.Entities.Carts;
using GrowStore.Domain.Interfaces;
using Moq;

namespace Growstore.Tests.UnitTests.Services;

public class CartServiceTests
{
    private readonly Mock<ICartRepository> _cartRepoMock;
    private readonly Mock<IValidator<CreateCartItemDto>> _createValidatorMock;
    private readonly Mock<IValidator<UpdateCartItemDto>> _updateValidatorMock;
    private readonly CartService _cartService;

    public CartServiceTests()
    {
        _cartRepoMock = new Mock<ICartRepository>();
        _createValidatorMock = new Mock<IValidator<CreateCartItemDto>>();
        _updateValidatorMock = new Mock<IValidator<UpdateCartItemDto>>();

        _createValidatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<CreateCartItemDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _updateValidatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<UpdateCartItemDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _cartService = new CartService(
            _cartRepoMock.Object,
            _createValidatorMock.Object,
            _updateValidatorMock.Object);
    }

    [Fact]
    public async Task GetCartAsync_EmptyUserId_ReturnsValidationError()
    {
        var result = await _cartService.GetCartAsync(Guid.Empty);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("Cart.InvalidUserId");
    }

    [Fact]
    public async Task GetCartAsync_NoExistingCart_CreatesNewCart()
    {
        var userId = Guid.NewGuid();
        _cartRepoMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync((Cart?)null);

        var result = await _cartService.GetCartAsync(userId);

        result.IsSuccess.Should().BeTrue();
        _cartRepoMock.Verify(r => r.AddAsync(It.IsAny<Cart>()), Times.Once);
    }

    [Fact]
    public async Task GetCartAsync_ExistingCart_ReturnsCart()
    {
        var userId = Guid.NewGuid();
        var cart = Cart.Create(userId);

        _cartRepoMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(cart);

        var result = await _cartService.GetCartAsync(userId);

        result.IsSuccess.Should().BeTrue();
        result.Value!.UserId.Should().Be(userId);
    }

    [Fact]
    public async Task AddItemAsync_EmptyUserId_ReturnsValidationError()
    {
        var dto = new CreateCartItemDto { ProductVariantId = Guid.NewGuid(), Quantity = 1 };

        var result = await _cartService.AddItemAsync(Guid.Empty, dto);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("Cart.InvalidUserId");
    }

    [Fact]
    public async Task AddItemAsync_WithInvalidDto_ReturnsValidationError()
    {
        var userId = Guid.NewGuid();
        var dto = new CreateCartItemDto();
        var errors = new List<ValidationFailure> { new("Quantity", "Quantity is required.") };

        _createValidatorMock
            .Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(errors));

        var result = await _cartService.AddItemAsync(userId, dto);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("CartItem.Validation");
    }

    [Fact]
    public async Task AddItemAsync_ProductVariantNotFound_ReturnsNotFound()
    {
        var userId = Guid.NewGuid();
        var dto = new CreateCartItemDto { ProductVariantId = Guid.NewGuid(), Quantity = 1 };

        _cartRepoMock.Setup(r => r.ProductVariantExistsAsync(dto.ProductVariantId)).ReturnsAsync(false);

        var result = await _cartService.AddItemAsync(userId, dto);

        result.IsFailure.Should().BeTrue();
        result.Error!.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task UpdateItemAsync_EmptyUserId_ReturnsValidationError()
    {
        var dto = new UpdateCartItemDto { ProductVariantId = Guid.NewGuid(), Quantity = 2 };

        var result = await _cartService.UpdateItemAsync(Guid.Empty, dto);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("Cart.InvalidUserId");
    }

    [Fact]
    public async Task UpdateItemAsync_CartNotFound_ReturnsNotFound()
    {
        var userId = Guid.NewGuid();
        var dto = new UpdateCartItemDto { ProductVariantId = Guid.NewGuid(), Quantity = 2 };

        _cartRepoMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync((Cart?)null);

        var result = await _cartService.UpdateItemAsync(userId, dto);

        result.IsFailure.Should().BeTrue();
        result.Error!.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task RemoveItemAsync_EmptyUserId_ReturnsValidationError()
    {
        var result = await _cartService.RemoveItemAsync(Guid.Empty, Guid.NewGuid());

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("Cart.InvalidUserId");
    }

    [Fact]
    public async Task RemoveItemAsync_CartNotFound_ReturnsNotFound()
    {
        var userId = Guid.NewGuid();
        _cartRepoMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync((Cart?)null);

        var result = await _cartService.RemoveItemAsync(userId, Guid.NewGuid());

        result.IsFailure.Should().BeTrue();
        result.Error!.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task ClearCartAsync_EmptyUserId_ReturnsValidationError()
    {
        var result = await _cartService.ClearCartAsync(Guid.Empty);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("Cart.InvalidUserId");
    }

    [Fact]
    public async Task ClearCartAsync_ValidUserId_ReturnsSuccess()
    {
        var userId = Guid.NewGuid();

        var result = await _cartService.ClearCartAsync(userId);

        result.IsSuccess.Should().BeTrue();
        _cartRepoMock.Verify(r => r.DeleteAsync(userId), Times.Once);
    }
}
