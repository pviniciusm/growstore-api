using FluentAssertions;
using GrowStore.Application.Shared.Exceptions;
using GrowStore.Application.Users.DTOs;
using GrowStore.Application.Users.Services;
using GrowStore.Domain.Entities;
using GrowStore.Domain.Interfaces;
using GrowStore.Domain.Shared.Enums;
using Moq;

namespace Growstore.Tests.UnitTests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _userService = new UserService(_userRepositoryMock.Object);
        }

        [Fact]
        public async Task CreateUserAsync_ValidData_ShouldReturnResponseUserDto()
        {
            // Arrange
            var dto = new CreateUserDto
            {
                Name = "John Doe",
                Cpf = "123.456.789-00",
                BirthDate = new DateTime(1990, 1, 1),
                Role = UserRole.CUSTOMER
            };

            _userRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _userService.CreateUserAsync(dto);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be(dto.Name);
            result.Cpf.Should().Be(dto.Cpf);
            result.BirthDate.Should().Be(dto.BirthDate);
            result.Role.Should().Be(dto.Role);
            _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task GetUserByIdAsync_ExistingId_ShouldReturnResponseUserDto()
        {
            // Arrange
            var user = User.Create("John Doe", "123.456.789-00", new DateTime(1990, 1, 1), UserRole.CUSTOMER);

            _userRepositoryMock
                .Setup(r => r.GetUserByIdAsync(user.Id))
                .ReturnsAsync(user);

            // Act
            var result = await _userService.GetUserByIdAsync(user.Id);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(user.Id);
            result.Name.Should().Be(user.Name);
            result.Cpf.Should().Be(user.Cpf);
        }

        [Fact]
        public async Task GetUserByIdAsync_NonExistingId_ShouldThrowNotFoundException()
        {
            // Arrange
            var id = Guid.NewGuid();

            _userRepositoryMock
                .Setup(r => r.GetUserByIdAsync(id))
                .ReturnsAsync((User?)null);

            // Act
            var act = async () => await _userService.GetUserByIdAsync(id);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage($"User not found by ID: {id}");
        }

        [Fact]
        public async Task UpdateUserAsync_ExistingId_ShouldCallRepositoryUpdate()
        {
            // Arrange
            var user = User.Create("John Doe", "123.456.789-00", new DateTime(1990, 1, 1), UserRole.CUSTOMER);

            var dto = new UpdateUserDto
            {
                Name = "John Smith",
                Cpf = "111.222.333-44",
                BirthDate = new DateTime(1992, 8, 20),
                Role = UserRole.ADMIN
            };

            _userRepositoryMock
                .Setup(r => r.GetUserByIdAsync(user.Id))
                .ReturnsAsync(user);

            _userRepositoryMock
                .Setup(r => r.UpdateAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            // Act
            await _userService.UpdateUserAsync(user.Id, dto);

            // Assert
            _userRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task UpdateUserAsync_NonExistingId_ShouldThrowNotFoundException()
        {
            // Arrange
            var id = Guid.NewGuid();

            _userRepositoryMock
                .Setup(r => r.GetUserByIdAsync(id))
                .ReturnsAsync((User?)null);

            var act = async () => await _userService.UpdateUserAsync(id, new UpdateUserDto());

            // Act & Assert
            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage($"User not found by ID: {id}");
        }

        [Fact]
        public async Task DeleteUserAsync_ExistingId_ShouldCallRepositoryDelete()
        {
            // Arrange
            var user = User.Create("John Doe", "123.456.789-00", new DateTime(1990, 1, 1), UserRole.CUSTOMER);

            _userRepositoryMock
                .Setup(r => r.GetUserByIdAsync(user.Id))
                .ReturnsAsync(user);

            _userRepositoryMock
                .Setup(r => r.DeleteAsync(user.Id))
                .Returns(Task.CompletedTask);

            // Act
            await _userService.DeleteUserAsync(user.Id);

            // Assert
            _userRepositoryMock.Verify(r => r.DeleteAsync(user.Id), Times.Once);
        }

        [Fact]
        public async Task DeleteUserAsync_NonExistingId_ShouldThrowNotFoundException()
        {
            // Arrange
            var id = Guid.NewGuid();

            _userRepositoryMock
                .Setup(r => r.GetUserByIdAsync(id))
                .ReturnsAsync((User?)null);

            var act = async () => await _userService.DeleteUserAsync(id);

            // Act & Assert
            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage($"User not found by ID: {id}");
        }
    }
}
