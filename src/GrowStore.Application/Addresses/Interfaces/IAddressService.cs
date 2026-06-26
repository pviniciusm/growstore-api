using GrowStore.Domain.Entities.Addresses;
using System;
using System.Threading.Tasks;

namespace GrowStore.Application.Addresses.Interfaces;

public interface IAddressService
{
    Task<Address?> GetByIdAsync(Guid id);
    
    // TODO: Atribuir DTOs no lugar de entidades, para evitar expor detalhes desnecessários.
    // Task<Address> AddAsync(Address address);
    // Task<IEnumerable<Address>> GetByUserIdAsync(Guid userId);
    // Task UpdateAsync(Address address);
    // Task DeleteAsync(Guid id);
    // Task SetDefaultAsync(Guid id, Guid userId);
}