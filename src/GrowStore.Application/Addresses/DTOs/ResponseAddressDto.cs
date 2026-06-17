using System;

namespace GrowStore.Application.Addresses.DTOs;

public class AddressResponseDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public bool IsDefault { get; set; }
    public string? Reference { get; set; }
    public string ZipCode { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
}