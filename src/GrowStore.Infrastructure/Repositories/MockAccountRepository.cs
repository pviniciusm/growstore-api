using GrowStore.Domain.Interfaces;
using GrowStore.Domain.Entities.Accounts;

namespace GrowStore.Infrastructure.Repositories;

public class MockAccountRepository : IAccountRepository
{
    private readonly List<Account> _accounts = [];

    public Task AddAsync(Account account)
    {
        if (_accounts.Any(x => x.Id == account.Id))
        {
            throw new InvalidOperationException("Account already exists.");
        }

        _accounts.Add(account);

        return Task.CompletedTask;
    }

    public Task<Account?> GetByEmailAsync(string email)
    {
        var account = _accounts.FirstOrDefault(x => x.Email == email);

        return Task.FromResult(account);
    }

    public Task<Account?> GetByIdAsync(Guid accountId)
    {
        var account = _accounts.FirstOrDefault(x => x.Id == accountId);

        return Task.FromResult(account);
    }

    public Task<bool> EmailExistsAsync(string email)
    {
        var exists = _accounts.Any(x => x.Email == email);

        return Task.FromResult(exists);
    }

    public Task UpdateAsync(Account account)
    {
        var index = _accounts.FindIndex(x => x.Id == account.Id);

        if (index == -1)
        {
            throw new InvalidOperationException("Account not found.");
        }

        _accounts[index] = account;

        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid accountId)
    {
        var account = _accounts.FirstOrDefault(x => x.Id == accountId);

        if (account is not null)
        {
            _accounts.Remove(account);
        }

        return Task.CompletedTask;
    }
}