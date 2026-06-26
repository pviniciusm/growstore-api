using GrowStore.Domain.Entities.Accounts;
using GrowStore.Domain.Interfaces;
using GrowStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GrowStore.Infrastructure.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly GrowStoreDbContext _context;

    public AccountRepository(GrowStoreDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Account account)
    {
        await _context.Accounts.AddAsync(account);
        await _context.SaveChangesAsync();
    }

    public async Task<Account?> GetByEmailAsync(string email)
    {
        return await _context.Accounts.FirstOrDefaultAsync(a => a.Email == email);
    }

    public async Task<Account?> GetByIdAsync(Guid accountId)
    {
        return await _context.Accounts.FirstOrDefaultAsync(a => a.Id == accountId);
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _context.Accounts.AnyAsync(a => a.Email == email);
    }

    public async Task UpdateAsync(Account account)
    {
        _context.Accounts.Update(account);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid accountId)
    {
        var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == accountId);

        if (account is null)
        {
            return;
        }

        _context.Accounts.Remove(account);
        await _context.SaveChangesAsync();
    }
}