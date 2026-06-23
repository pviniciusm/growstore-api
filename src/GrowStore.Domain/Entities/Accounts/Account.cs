using GrowStore.Domain.Shared.Exceptions;

namespace GrowStore.Domain.Entities.Accounts;

public class Account
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string Password { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public Account()
    {
        //Necessário para o Entity Framework
    }

    public Account(Guid userId, string email, string password)
    {
        if (userId == Guid.Empty)
            throw new DomainException("UserId is required.");
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email is required.");
        if (string.IsNullOrWhiteSpace(password))
            throw new DomainException("Password is required.");

        Id = Guid.NewGuid();
        UserId = userId;
        Email = email;
        Password = password;
    }
}