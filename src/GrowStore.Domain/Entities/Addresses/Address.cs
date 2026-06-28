using GrowStore.Domain.Shared;
using GrowStore.Domain.Shared.Exceptions;

namespace GrowStore.Domain.Entities.Addresses;

public class Address
{
    public Address()
    {
        // Necessário para o Entity Framework
    }

    public Address(Guid userId, string street, string city, string state, string zipCode, bool isDefault, string? reference = null)
    {
        if (userId == Guid.Empty)
            throw new DomainException("UserId is required.");

        if (string.IsNullOrWhiteSpace(street))
            throw new DomainException("Street is required.");

        if (string.IsNullOrWhiteSpace(city))
            throw new DomainException("City is required.");

        if (string.IsNullOrWhiteSpace(state))
            throw new DomainException("State is required.");

        if (string.IsNullOrWhiteSpace(zipCode))
            throw new DomainException("ZipCode is required.");

        Id = Guid.NewGuid();
        UserId = userId;

        Street = street;
        City = city;
        State = state;
        ZipCode = zipCode;
        Reference = reference;

        IsDefault = isDefault;

        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Street { get; private set; } = string.Empty;
    public string City { get; private set; } = string.Empty;
    public string State { get; private set; } = string.Empty;
    public string ZipCode { get; private set; } = string.Empty;
    public string? Reference { get; private set; }
    public bool IsDefault { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public void Update(string street, string city, string state, string zipCode, bool isDefault, string? reference)
    {
        if (string.IsNullOrWhiteSpace(street))
            throw new DomainException("Street is required.");

        if (string.IsNullOrWhiteSpace(city))
            throw new DomainException("City is required.");

        if (string.IsNullOrWhiteSpace(state))
            throw new DomainException("State is required.");

        if (string.IsNullOrWhiteSpace(zipCode))
            throw new DomainException("ZipCode is required.");

        Street = street;
        City = city;
        State = state;
        ZipCode = zipCode;
        Reference = reference;
        IsDefault = isDefault;

        UpdatedAt = DateTime.UtcNow;
    }
}