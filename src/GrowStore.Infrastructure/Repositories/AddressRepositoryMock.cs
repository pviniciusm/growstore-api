using GrowStore.Domain.Entities.Addresses;
using GrowStore.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrowStore.Infrastructure.Repositories;

public class AddressRepositoryMock : IAddressRepository
{
    private readonly List<Address> _addresses = new();

    public async Task<Address?> GetByIdAsync(Guid id)
    {
        var address = _addresses.FirstOrDefault(a => a.Id == id);
        return await Task.FromResult(address);
    }

    public async Task AddAsync(Address address)
    {
        _addresses.Add(address);
        await Task.CompletedTask;
    }

    public async Task<IEnumerable<Address>> GetByUserIdAsync(Guid userId)
    {
        var userAddresses = _addresses.Where(a => a.UserId == userId);
        return await Task.FromResult(userAddresses);
    }

    public async Task UpdateAsync(Address address)
    {
        var existing = _addresses.FirstOrDefault(a => a.Id == address.Id);
        if (existing != null)
        {
            _addresses.Remove(existing);
            _addresses.Add(address);
        }
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id)
    {
        var existing = _addresses.FirstOrDefault(a => a.Id == id);
        if (existing != null)
        {
            _addresses.Remove(existing);
        }
        await Task.CompletedTask;
    }
}