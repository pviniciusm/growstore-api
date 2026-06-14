using GrowStore.Domain.Shared;

namespace GrowStore.Domain.Entities
{
    public class User
    {
        private User()
        {
            //Necessário para EF
        }

        private User (string name, string email, UserRole role)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new DomainException("Name is required.");
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                throw new DomainException("Email is required.");
            }

            if (!Enum.IsDefined(typeof(UserRole), role)) throw new DomainException("Invalid role.");

            Name = name;
            Email = email;
            Role = role;
        }
        public Guid Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public UserRole Role { get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime UpdateAt { get; private set; }
        public bool IsActive { get; private set; }
    }
}