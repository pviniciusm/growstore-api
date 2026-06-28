using GrowStore.Application.Common.Results;
using GrowStore.Application.Products.DTOs;

namespace GrowStore.Application.Products.Interfaces;

public interface IProductService
{
    Task<Result<ResponseProductDto>> CreateAsync(CreateProductDto dto);
    Task<Result<IEnumerable<ResponseProductDto>>> GetAllAsync();
    Task<Result<ResponseProductDto>> GetByIdAsync(Guid id);
    Task<Result<ResponseProductDto>> UpdateAsync(Guid id, UpdateProductDto dto);
    Task<Result> DeleteAsync(Guid id);
}
