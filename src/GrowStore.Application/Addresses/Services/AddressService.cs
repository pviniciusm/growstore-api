using GrowStore.Application.Addresses.Interfaces;
using GrowStore.Domain.Entities.Addresses;
using GrowStore.Domain.Interfaces;
using System;
using System.Threading.Tasks;

namespace GrowStore.Application.Addresses.Services;

public class AddressService : IAddressService
{
    private readonly IAddressRepository _addressRepository;

    public AddressService(IAddressRepository addressRepository)
    {
        _addressRepository = addressRepository;
    }

    public async Task<Address?> GetByIdAsync(Guid id)
    {
        return await _addressRepository.GetByIdAsync(id);
    }
}