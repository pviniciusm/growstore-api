using GrowStore.Domain.Entities;
using GrowStore.Domain.Interfaces;

namespace GrowStore.Infrastructure.Repositories;

public class AddressRepositoryMock : IAddressRepository
{
    // Simulação de uma lista em memória
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
}