using FluentAssertions;
using GrowStore.Domain.Entities;
using GrowStore.Domain.Shared.Enums;
using GrowStore.Domain.Shared.Exceptions;

namespace Growstore.Tests.UnitTests.Entities
{
    public class UserTests
    {
        [Fact]
        public void Constructor_Default_ShouldCreateInstanceWithDefaultValues()
        {
            // Arrange & Act
            var user = (User)Activator.CreateInstance(typeof(User), nonPublic: true)!;

            // Assert
            user.Should().NotBeNull();
            user.Id.Should().Be(Guid.Empty);
            user.Name.Should().Be(string.Empty);
            user.Cpf.Should().Be(string.Empty);
            user.IsActive.Should().BeFalse();
        }

        [Fact]
        public void Create_ValidData_ShouldReturnUserWithCorrectProperties()
        {
            // Arrange
            var name = "John Doe";
            var cpf = "123.456.789-09";
            var birthDate = new DateTime(1990, 5, 15);
            var role = UserRole.CUSTOMER;

            // Act
            var user = User.Create(name, cpf, birthDate, role);

            // Assert
            user.Id.Should().NotBe(Guid.Empty);
            user.Name.Should().Be(name);
            user.Cpf.Should().Be(cpf);
            user.BirthDate.Should().Be(birthDate);
            user.Role.Should().Be(role);
            user.IsActive.Should().BeTrue();
            user.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_InvalidName_ShouldThrowDomainException(string name)
        {
            // Arrange
            var act = () => User.Create(name!, "123.456.789-09", new DateTime(1990, 1, 1), UserRole.CUSTOMER);

            // Act & Assert
            act.Should().Throw<DomainException>().WithMessage("Name is required.");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_InvalidCpf_ShouldThrowDomainException(string cpf)
        {
            // Arrange
            var act = () => User.Create("John Doe", cpf!, new DateTime(1990, 1, 1), UserRole.CUSTOMER);

            // Act & Assert
            act.Should().Throw<DomainException>().WithMessage("Cpf is required.");
        }

        [Fact]
        public void Create_MinValueBirthDate_ShouldThrowDomainException()
        {
            // Arrange
            var act = () => User.Create("John Doe", "123.456.789-09", DateTime.MinValue, UserRole.CUSTOMER);

            // Act & Assert
            act.Should().Throw<DomainException>().WithMessage("BirthDate is required.");
        }

        [Fact]
        public void Create_InvalidRole_ShouldThrowDomainException()
        {
            // Arrange
            var act = () => User.Create("John Doe", "123.456.789-09", new DateTime(1990, 1, 1), (UserRole)999);

            // Act & Assert
            act.Should().Throw<DomainException>().WithMessage("Invalid role.");
        }

        [Fact]
        public void Update_ValidData_ShouldUpdateProperties()
        {
            // Arrange
            var user = User.Create("John Doe", "123.456.789-09", new DateTime(1990, 1, 1), UserRole.CUSTOMER);

            // Act
            user.Update("John Smith", "111.222.333-44", new DateTime(1992, 8, 20), UserRole.ADMIN);

            // Assert
            user.Name.Should().Be("John Smith");
            user.Cpf.Should().Be("111.222.333-44");
            user.BirthDate.Should().Be(new DateTime(1992, 8, 20));
            user.Role.Should().Be(UserRole.ADMIN);
            user.UpdateAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void Update_InvalidData_ShouldNotChangeOriginalState()
        {
            // Arrange
            var user = User.Create("John Doe", "123.456.789-09", new DateTime(1990, 1, 1), UserRole.CUSTOMER);

            // Act
            var act = () => user.Update(string.Empty, "123.456.789-09", new DateTime(1990, 1, 1), UserRole.CUSTOMER);

            // Assert
            act.Should().Throw<DomainException>();
            user.Name.Should().Be("John Doe");
            user.Cpf.Should().Be("123.456.789-09");
            user.BirthDate.Should().Be(new DateTime(1990, 1, 1));
            user.Role.Should().Be(UserRole.CUSTOMER);
        }
    }
}
