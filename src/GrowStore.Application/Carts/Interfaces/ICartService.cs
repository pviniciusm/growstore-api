using GrowStore.Application.Carts.DTOs;
using GrowStore.Application.Common.Results;

namespace GrowStore.Application.Carts.Interfaces;

public interface ICartService
{
    Task<Result<ResponseCartDto>> GetCartAsync(Guid userId);
    Task<Result<ResponseCartDto>> AddItemAsync(Guid userId, CreateCartItemDto dto);
    Task<Result<ResponseCartDto>> UpdateItemAsync(Guid userId, UpdateCartItemDto dto);
    Task<Result> RemoveItemAsync(Guid userId, Guid productVariantId);
    Task<Result> ClearCartAsync(Guid userId);
}