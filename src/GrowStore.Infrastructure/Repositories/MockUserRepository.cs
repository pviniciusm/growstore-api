using GrowStore.Domain.Entities;
using GrowStore.Domain.Interfaces;

namespace GrowStore.Infrastructure.Repositories;

public class MockUserRepository : IUserRepository
{
    private static readonly List<User> _users = [];

    public Task AddAsync(User user)
    {
        _users.Add(user);

        return Task.CompletedTask;
    }

    public Task<User?> GetUserByIdAsync(Guid id)
    {
        var user = _users.FirstOrDefault(x => x.Id == id);

        return Task.FromResult(user);
    }

    public Task UpdateAsync(User user)
    {
        var index = _users.FindIndex(x => x.Id == user.Id);

        if (index == -1)
        {
            throw new InvalidOperationException("User not found.");
        }

        _users[index] = user;

        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id)
    {
        var user = _users.FirstOrDefault(x => x.Id == id);

        if (user is not null)
        {
            _users.Remove(user);
        }

        return Task.CompletedTask;
    }
}