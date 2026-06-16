using GrowStore.Domain.Shared;

namespace GrowStore.Application.Accounts.DTOs;

public class UpdateAccountDto
{
    public string Email { get; set; } = string.Empty;
    public UserRole Role { get; set; }
}