using GrowStore.Application.Addresses.DTOs;
using GrowStore.Application.Addresses.Interfaces;
using GrowStore.Application.Common.Errors;
using GrowStore.Application.Common.Results;
using GrowStore.Domain.Entities.Addresses;
using GrowStore.Domain.Interfaces;

namespace GrowStore.Application.Addresses.Services;

public class AddressService : IAddressService
{
    private readonly IAddressRepository _addressRepository;

    public AddressService(IAddressRepository addressRepository)
    {
        _addressRepository = addressRepository;
    }

    public async Task<Result<AddressResponseDto>> CreateAsync(CreateAddressDto dto)
    {
        var address = new Address(dto.UserId, dto.Street, dto.City, dto.State, dto.ZipCode, dto.IsDefault, dto.Reference);
        await _addressRepository.AddAsync(address);

        return Result<AddressResponseDto>.Success(MapToResponse(address));
    }

    public async Task<Result<AddressResponseDto>> GetByIdAsync(Guid id)
    {
        var address = await _addressRepository.GetByIdAsync(id);

        if (address is null)
        {
            return Result<AddressResponseDto>.Failure(
                Error.NotFound("Address.NotFound", "Address not found."));
        }

        return Result<AddressResponseDto>.Success(MapToResponse(address));
    }

    public async Task<Result<IEnumerable<AddressResponseDto>>> GetByUserIdAsync(Guid userId)
    {
        var addresses = await _addressRepository.GetByUserIdAsync(userId);
        var response = addresses.Select(MapToResponse);

        return Result<IEnumerable<AddressResponseDto>>.Success(response);
    }

    public async Task<Result<AddressResponseDto>> UpdateAsync(Guid id, UpdateAddressDto dto)
    {
        var address = await _addressRepository.GetByIdAsync(id);

        if (address is null)
        {
            return Result<AddressResponseDto>.Failure(
                Error.NotFound("Address.NotFound", "Address not found."));
        }

        address.Update(dto.Street, dto.City, dto.State, dto.ZipCode, dto.IsDefault, dto.Reference);
        await _addressRepository.UpdateAsync(address);

        return Result<AddressResponseDto>.Success(MapToResponse(address));
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var address = await _addressRepository.GetByIdAsync(id);

        if (address is null)
        {
            return Result.Failure(
                Error.NotFound("Address.NotFound", "Address not found."));
        }

        await _addressRepository.DeleteAsync(id);

        return Result.Success();
    }

    private static AddressResponseDto MapToResponse(Address address) => new()
    {
        Id = address.Id,
        UserId = address.UserId,
        IsDefault = address.IsDefault,
        Reference = address.Reference,
        ZipCode = address.ZipCode,
        State = address.State,
        City = address.City,
        Street = address.Street
    };
}