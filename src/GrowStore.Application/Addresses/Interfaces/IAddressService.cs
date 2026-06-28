using GrowStore.Application.Addresses.DTOs;
using GrowStore.Application.Common.Results;

namespace GrowStore.Application.Addresses.Interfaces;

public interface IAddressService
{
    Task<Result<AddressResponseDto>> CreateAsync(CreateAddressDto dto);
    Task<Result<AddressResponseDto>> GetByIdAsync(Guid id);
    Task<Result<IEnumerable<AddressResponseDto>>> GetByUserIdAsync(Guid userId);
    Task<Result<AddressResponseDto>> UpdateAsync(Guid id, UpdateAddressDto dto);
    Task<Result> DeleteAsync(Guid id);

    // Task SetDefaultAsync(Guid id, Guid userId);
}