using GrowStore.Domain.Entities.Accounts;

namespace GrowStore.Domain.Interfaces;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken refreshToken);
    Task<RefreshToken?> GetByTokenAsync(string token);
    Task UpdateAsync(RefreshToken refreshToken);
}