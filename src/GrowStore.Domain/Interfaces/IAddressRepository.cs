using GrowStore.Domain.Entities.Addresses;

namespace GrowStore.Domain.Interfaces;

public interface IAddressRepository
{
    Task AddAsync(Address address);
    Task<Address?> GetByIdAsync(Guid id);
    Task<IEnumerable<Address>> GetByUserIdAsync(Guid userId);
    Task UpdateAsync(Address address);
    Task DeleteAsync(Guid id);
}