using GrowStore.Domain.Entities.Addresses;
using GrowStore.Domain.Interfaces;
using GrowStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GrowStore.Infrastructure.Repositories;

public class AddressRepository : IAddressRepository
{
    private readonly GrowStoreDbContext _context;

    public AddressRepository(GrowStoreDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Address address)
    {
        await _context.Addresses.AddAsync(address);
        await _context.SaveChangesAsync();
    }

    public async Task<Address?> GetByIdAsync(Guid id)
    {
        return await _context.Addresses.FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<Address>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Addresses
            .Where(a => a.UserId == userId)
            .ToListAsync();
    }

    public async Task UpdateAsync(Address address)
    {
        _context.Addresses.Update(address);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var address = await _context.Addresses.FirstOrDefaultAsync(a => a.Id == id);

        if (address is null)
        {
            return;
        }

        _context.Addresses.Remove(address);
        await _context.SaveChangesAsync();
    }
}