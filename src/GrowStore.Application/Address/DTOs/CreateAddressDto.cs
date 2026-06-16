using System;

namespace GrowStore.Application.Addresses.DTOs;

public class CreateAddressDto
{
    public Guid UserId { get; set; }
    public bool IsDefault { get; set; }
    public string? Reference { get; set; }
    public string ZipCode { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty; // 2-digit acronym (e.g., "SP")
    public string City { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
}