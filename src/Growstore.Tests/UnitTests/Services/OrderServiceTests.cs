using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using GrowStore.Application.Orders.DTOs;
using GrowStore.Application.Orders.Services;
using GrowStore.Domain.Entities.Orders;
using GrowStore.Domain.Entities.Products;
using GrowStore.Domain.Interfaces;
using GrowStore.Domain.Shared.Enums;
using Moq;

namespace Growstore.Tests.UnitTests.Services;

public class OrderServiceTests
{
    private readonly Mock<IOrderRepository> _orderRepoMock;
    private readonly Mock<IProductRepository> _productRepoMock;
    private readonly Mock<IValidator<CreateOrderDto>> _createValidatorMock;
    private readonly Mock<IValidator<UpdateOrderStatusDto>> _updateStatusValidatorMock;
    private readonly OrderService _orderService;

    public OrderServiceTests()
    {
        _orderRepoMock = new Mock<IOrderRepository>();
        _productRepoMock = new Mock<IProductRepository>();
        _createValidatorMock = new Mock<IValidator<CreateOrderDto>>();
        _updateStatusValidatorMock = new Mock<IValidator<UpdateOrderStatusDto>>();

        _createValidatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<CreateOrderDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _updateStatusValidatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<UpdateOrderStatusDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _orderService = new OrderService(
            _orderRepoMock.Object,
            _productRepoMock.Object,
            _createValidatorMock.Object,
            _updateStatusValidatorMock.Object);
    }

    // === CreateAsync ===

    [Fact]
    public async Task CreateAsync_EmptyUserId_ReturnsValidationError()
    {
        var dto = new CreateOrderDto { Items = [new CreateOrderItemDto { ProductVariantId = Guid.NewGuid(), Quantity = 1 }] };

        var result = await _orderService.CreateAsync(Guid.Empty, dto);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("Order.InvalidUserId");
    }

    [Fact]
    public async Task CreateAsync_EmptyItems_ReturnsValidationError()
    {
        var dto = new CreateOrderDto();

        _createValidatorMock
            .Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new List<ValidationFailure>
            {
                new("Items", "Order must have at least one item.")
            }));

        var result = await _orderService.CreateAsync(Guid.NewGuid(), dto);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("Order.Validation");
    }

    [Fact]
    public async Task CreateAsync_VariantNotFound_ReturnsNotFound()
    {
        var variantId = Guid.NewGuid();
        var dto = new CreateOrderDto
        {
            Items = [new CreateOrderItemDto { ProductVariantId = variantId, Quantity = 1 }]
        };

        _productRepoMock.Setup(r => r.GetVariantByIdAsync(variantId)).ReturnsAsync((ProductVariant?)null);

        var result = await _orderService.CreateAsync(Guid.NewGuid(), dto);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("ProductVariant.NotFound");
    }

    [Fact]
    public async Task CreateAsync_InsufficientStock_ReturnsNotFound()
    {
        var variantId = Guid.NewGuid();
        var dto = new CreateOrderDto
        {
            Items = [new CreateOrderItemDto { ProductVariantId = variantId, Quantity = 10 }]
        };

        _productRepoMock.Setup(r => r.GetVariantByIdAsync(variantId)).ReturnsAsync((ProductVariant?)null);

        var result = await _orderService.CreateAsync(Guid.NewGuid(), dto);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("ProductVariant.NotFound");
    }

    // === GetByIdAsync ===

    [Fact]
    public async Task GetByIdAsync_ExistingOrder_ReturnsOrder()
    {
        var order = Order.Create(Guid.NewGuid());
        order.AddItem(Guid.NewGuid(), "Planta", null, 2, 25.00m);

        _orderRepoMock.Setup(r => r.GetByIdAsync(order.Id)).ReturnsAsync(order);

        var result = await _orderService.GetByIdAsync(order.Id);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Id.Should().Be(order.Id);
        result.Value.Items.Should().HaveCount(1);
        result.Value.TotalAmount.Should().Be(50.00m);
    }

    [Fact]
    public async Task GetByIdAsync_NonExisting_ReturnsNotFound()
    {
        _orderRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Order?)null);

        var result = await _orderService.GetByIdAsync(Guid.NewGuid());

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("Order.NotFound");
    }

    // === GetByUserIdAsync ===

    [Fact]
    public async Task GetByUserIdAsync_ReturnsOrdersList()
    {
        var userId = Guid.NewGuid();
        var orders = new List<Order> { Order.Create(userId), Order.Create(userId) };

        _orderRepoMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(orders);

        var result = await _orderService.GetByUserIdAsync(userId);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Count().Should().Be(2);
    }

    // === UpdateStatusAsync ===

    [Fact]
    public async Task UpdateStatusAsync_ExistingOrder_ReturnsSuccess()
    {
        var order = Order.Create(Guid.NewGuid());
        var dto = new UpdateOrderStatusDto { Status = OrderStatus.CONFIRMED };

        _orderRepoMock.Setup(r => r.GetByIdAsync(order.Id)).ReturnsAsync(order);
        _orderRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Order>())).Returns(Task.CompletedTask);

        var result = await _orderService.UpdateStatusAsync(order.Id, dto);

        result.IsSuccess.Should().BeTrue();
        _orderRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Order>()), Times.Once);
    }

    [Fact]
    public async Task UpdateStatusAsync_NonExistingOrder_ReturnsNotFound()
    {
        var dto = new UpdateOrderStatusDto { Status = OrderStatus.SHIPPED };

        _orderRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Order?)null);

        var result = await _orderService.UpdateStatusAsync(Guid.NewGuid(), dto);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("Order.NotFound");
    }

    [Fact]
    public async Task UpdateStatusAsync_InvalidStatus_ReturnsValidationError()
    {
        var dto = new UpdateOrderStatusDto();

        _updateStatusValidatorMock
            .Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new List<ValidationFailure>
            {
                new("Status", "Invalid order status.")
            }));

        var result = await _orderService.UpdateStatusAsync(Guid.NewGuid(), dto);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("Order.Validation");
    }
}