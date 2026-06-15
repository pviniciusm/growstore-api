using GrowStore.Domain.Shared;

namespace GrowStore.Application.Accounts.DTOs;

public class CreateAccountDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.CUSTOMER;
}