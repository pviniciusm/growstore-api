using GrowStore.Domain.Shared.Enums;
using GrowStore.Domain.Shared.Exceptions;

namespace GrowStore.Domain.Entities
{
    public class User
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Cpf { get; private set; } = string.Empty;
        public DateTime BirthDate { get; private set; }
        public UserRole Role { get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime UpdateAt { get; private set; }
        public bool IsActive { get; private set; }

        private User()
        {
            //Necessário para o Entity Framework
        }

        private User(string name, string cpf, DateTime birthDate, UserRole role)
        {
            Validate(name, cpf, birthDate, role);

            Name = name;
            Cpf = cpf;
            BirthDate = birthDate;
            Role = role;
        }

        public static User Create(string name, string cpf, DateTime birthDate, UserRole role)
        {
            var user = new User(name, cpf, birthDate, role);
            user.Id = Guid.NewGuid();
            user.IsActive = true;
            return user;
        }

        public void Update(string name, string cpf, DateTime birthDate, UserRole role)
        {
            Validate(name, cpf, birthDate, role);

            Name = name;
            Cpf = cpf;
            BirthDate = birthDate;
            Role = role;
            UpdateAt = DateTime.UtcNow;
        }

        private static void Validate(string name, string cpf, DateTime birthDate, UserRole role)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Name is required.");

            if (string.IsNullOrWhiteSpace(cpf))
                throw new DomainException("Cpf is required.");

            if (birthDate == DateTime.MinValue)
                throw new DomainException("BirthDate is required.");

            if (!Enum.IsDefined(typeof(UserRole), role))
                throw new DomainException("Invalid role.");
        }
    }
}
