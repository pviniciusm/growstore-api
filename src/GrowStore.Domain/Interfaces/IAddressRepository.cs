using GrowStore.Domain.Entities;

namespace GrowStore.Domain.Interfaces;

public interface IAddressRepository
{
    Task<Address?> GetByIdAsync(Guid id);
    Task AddAsync(Address address);
}