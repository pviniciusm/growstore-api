namespace GrowStore.Domain.Interfaces;

using GrowStore.Domain.Entities;

public interface IUserRepository
{
    Task AddAsync(User user);
    Task<User?> GetUserByIdAsync(Guid id);
    Task UpdateAsync(User user);
    Task DeleteAsync(Guid id);
}