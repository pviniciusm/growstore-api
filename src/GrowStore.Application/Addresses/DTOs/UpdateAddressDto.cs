namespace GrowStore.Application.Addresses.DTOs;

public class UpdateAddressDto
{
    public bool IsDefault { get; set; }
    public string? Reference { get; set; }
    public string ZipCode { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
}