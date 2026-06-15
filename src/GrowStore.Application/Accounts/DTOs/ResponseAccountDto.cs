using System;
using GrowStore.Domain.Shared;

namespace GrowStore.Application.Accounts.DTOs;

public class ResponseAccountDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public UserRole Role { get; set; }
}