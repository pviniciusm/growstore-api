using GrowStore.Domain.Shared.Enums;

namespace GrowStore.Application.Auth.DTOs
{
    public class TokenRequestDto
    {
        public Guid AccountId { get; set; }
        public Guid UserId { get; set; }
        public required string Email { get; set; }
        public UserRole Role { get; set; }
    }
}
