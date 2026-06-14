using GrowStore.Domain.Shared;

namespace GrowStore.Application.Users.DTOs;
    public class ResponseUserDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Cpf { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public UserRole Role { get; set; }
    }
