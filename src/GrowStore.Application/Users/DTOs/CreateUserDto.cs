using GrowStore.Domain.Shared;

namespace GrowStore.Application.Users.DTOs;
    public class CreateUserDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public UserRole Role { get; set; }
    }