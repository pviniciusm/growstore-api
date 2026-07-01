using GrowStore.Domain.Entities.Accounts;
using GrowStore.Domain.Interfaces;
using GrowStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GrowStore.Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly GrowStoreDbContext _context;

    public RefreshTokenRepository(GrowStoreDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(RefreshToken refreshToken)
    {
        await _context.RefreshTokens.AddAsync(refreshToken);
        await _context.SaveChangesAsync();
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        return await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token);
    }

    public async Task UpdateAsync(RefreshToken refreshToken)
    {
        _context.RefreshTokens.Update(refreshToken);
        await _context.SaveChangesAsync();
    }
}