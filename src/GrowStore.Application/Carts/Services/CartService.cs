using FluentValidation;
using GrowStore.Application.Carts.DTOs;
using GrowStore.Application.Carts.Interfaces;
using GrowStore.Application.Common.Errors;
using GrowStore.Application.Common.Results;
using GrowStore.Domain.Entities.Carts;
using GrowStore.Domain.Interfaces;

namespace GrowStore.Application.Carts.Services;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;
    private readonly IValidator<CreateCartItemDto> _createItemValidator;
    private readonly IValidator<UpdateCartItemDto> _updateItemValidator;

    public CartService(
        ICartRepository cartRepository,
        IValidator<CreateCartItemDto> createItemValidator,
        IValidator<UpdateCartItemDto> updateItemValidator)
    {
        _cartRepository = cartRepository;
        _createItemValidator = createItemValidator;
        _updateItemValidator = updateItemValidator;
    }

    public async Task<Result<ResponseCartDto>> GetCartAsync(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            return Result<ResponseCartDto>.Failure(
                Error.Validation("Cart.InvalidUserId", "User ID is required."));
        }

        var cart = await _cartRepository.GetByUserIdAsync(userId);

        if (cart == null)
        {
            cart = Cart.Create(userId);
            await _cartRepository.AddAsync(cart);
        }

        return Result<ResponseCartDto>.Success(MapToResponseDto(cart));
    }

    public async Task<Result<ResponseCartDto>> AddItemAsync(Guid userId, CreateCartItemDto dto)
    {
        if (userId == Guid.Empty)
        {
            return Result<ResponseCartDto>.Failure(
                Error.Validation("Cart.InvalidUserId", "User ID is required."));
        }

        var validationResult = await _createItemValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            var errorMessage = string.Join(" | ", validationResult.Errors.Select(e => e.ErrorMessage));
            return Result<ResponseCartDto>.Failure(
                Error.Validation("CartItem.Validation", errorMessage));
        }

        if (!await _cartRepository.ProductVariantExistsAsync(dto.ProductVariantId))
        {
            return Result<ResponseCartDto>.Failure(
                Error.NotFound("ProductVariant.NotFound", "Product variant not found."));
        }

        try
        {
            var cart = await _cartRepository.GetByUserIdAsync(userId);

            if (cart == null)
            {
                cart = Cart.Create(userId);
                await _cartRepository.AddAsync(cart);
                cart = await _cartRepository.GetByUserIdAsync(userId);
            }

            var price = await _cartRepository.GetProductVariantPriceAsync(dto.ProductVariantId);
            cart.AddItem(dto.ProductVariantId, dto.Quantity);

            foreach (var item in cart.Items)
            {
                var itemPrice = await _cartRepository.GetProductVariantPriceAsync(item.ProductVariantId);
                item.SetPrice(itemPrice);
            }

            await _cartRepository.UpdateAsync(cart);

            var updatedCart = await _cartRepository.GetByUserIdAsync(userId);
            return Result<ResponseCartDto>.Success(MapToResponseDto(updatedCart));
        }
        catch (Exception ex)
        {
            return Result<ResponseCartDto>.Failure(
                Error.Failure("Cart.Error", ex.Message));
        }
    }

    public async Task<Result<ResponseCartDto>> UpdateItemAsync(Guid userId, UpdateCartItemDto dto)
    {
        if (userId == Guid.Empty)
        {
            return Result<ResponseCartDto>.Failure(
                Error.Validation("Cart.InvalidUserId", "User ID is required."));
        }

        var validationResult = await _updateItemValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            var errorMessage = string.Join(" | ", validationResult.Errors.Select(e => e.ErrorMessage));
            return Result<ResponseCartDto>.Failure(
                Error.Validation("CartItem.Validation", errorMessage));
        }

        try
        {
            var cart = await _cartRepository.GetByUserIdAsync(userId);

            if (cart == null)
            {
                return Result<ResponseCartDto>.Failure(
                    Error.NotFound("Cart.NotFound", "Cart not found."));
            }

            cart.UpdateItemQuantity(dto.ProductVariantId, dto.Quantity);
            await _cartRepository.UpdateAsync(cart);

            var updatedCart = await _cartRepository.GetByUserIdAsync(userId);
            return Result<ResponseCartDto>.Success(MapToResponseDto(updatedCart));
        }
        catch (Exception ex)
        {
            return Result<ResponseCartDto>.Failure(
                Error.Failure("Cart.Error", ex.Message));
        }
    }

    public async Task<Result> RemoveItemAsync(Guid userId, Guid productVariantId)
    {
        if (userId == Guid.Empty)
        {
            return Result<ResponseCartDto>.Failure(
                Error.Validation("Cart.InvalidUserId", "User ID is required."));
        }

        if (productVariantId == Guid.Empty)
        {
            return Result<ResponseCartDto>.Failure(
                Error.Validation("CartItem.InvalidProductVariantId", "Product Variant ID is required."));
        }

        try
        {
            var cart = await _cartRepository.GetByUserIdAsync(userId);

            if (cart == null)
            {
                return Result<ResponseCartDto>.Failure(
                    Error.NotFound("Cart.NotFound", "Cart not found."));
            }

            if (!cart.Items.Any(i => i.ProductVariantId == productVariantId))
            {
                return Result<ResponseCartDto>.Failure(
                    Error.NotFound("CartItem.NotFound", "Item not found in cart."));
            }

            await _cartRepository.RemoveItemAsync(cart.Id, productVariantId);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result<ResponseCartDto>.Failure(
                Error.Failure("Cart.Error", ex.Message));
        }
    }

    public async Task<Result> ClearCartAsync(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            return Result.Failure(
                Error.Validation("Cart.InvalidUserId", "User ID is required."));
        }

        try
        {
            await _cartRepository.DeleteAsync(userId);
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(
                Error.Failure("Cart.Error", ex.Message));
        }
    }

    private static ResponseCartDto MapToResponseDto(Cart cart)
    {
        return new ResponseCartDto
        {
            Id = cart.Id,
            UserId = cart.UserId,
            Items = cart.Items.Select(item => new ResponseCartItemDto
            {
                ProductVariantId = item.ProductVariantId,
                Quantity = item.Quantity,
                Price = item.Price,
                Total = item.GetTotal(),
                Color = item.ProductVariant?.Color,
                Size = item.ProductVariant?.Size
            }).ToList(),
            Subtotal = cart.GetSubtotal(),
            CreatedAt = cart.CreatedAt,
            UpdatedAt = cart.UpdatedAt
        };
    }
}
