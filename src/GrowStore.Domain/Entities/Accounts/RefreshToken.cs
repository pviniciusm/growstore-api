using GrowStore.Domain.Shared.Exceptions;

namespace GrowStore.Domain.Entities.Accounts;

public class RefreshToken
{
    public Guid Id { get; private set; }
    public Guid AccountId { get; private set; }
    public string Token { get; private set; } = string.Empty;
    public DateTime ExpiresAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }

    private RefreshToken()
    {
        // Necessário para o Entity Framework
    }

    public RefreshToken(Guid accountId, string token, DateTime expiresAt)
    {
        if (accountId == Guid.Empty)
            throw new DomainException("AccountId is required.");

        if (string.IsNullOrWhiteSpace(token))
            throw new DomainException("Token is required.");

        Id = Guid.NewGuid();
        AccountId = accountId;
        Token = token;
        ExpiresAt = expiresAt;
        CreatedAt = DateTime.UtcNow;
    }

    public bool IsActive => RevokedAt is null && DateTime.UtcNow < ExpiresAt;

    public void Revoke() => RevokedAt = DateTime.UtcNow;
}