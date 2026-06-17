using System;
using GrowStore.Domain.Shared;
using GrowStore.Domain.Shared.Enums;

namespace GrowStore.Application.Accounts.DTOs;

public class ResponseAccountDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public UserRole Role { get; set; }
}