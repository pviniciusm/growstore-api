using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using GrowStore.Application.Common.Errors;
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
        private readonly Mock<IValidator<CreateUserDto>> _createUserValidatorMock;
        private readonly Mock<IValidator<UpdateUserDto>> _updateUserValidatorMock;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _createUserValidatorMock = new Mock<IValidator<CreateUserDto>>();
            _updateUserValidatorMock = new Mock<IValidator<UpdateUserDto>>();

            _createUserValidatorMock
                .Setup(v => v.ValidateAsync(It.IsAny<CreateUserDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _updateUserValidatorMock
                .Setup(v => v.ValidateAsync(It.IsAny<UpdateUserDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _userService = new UserService(
                _userRepositoryMock.Object,
                _createUserValidatorMock.Object,
                _updateUserValidatorMock.Object);
        }

        [Fact]
        public async Task CreateUserAsync_WithInvalidDto_ReturnsValidationError()
        {
            // Arrange
            var dto = new CreateUserDto();

            var validationErrors = new List<ValidationFailure>
            {
                new("Name", "Name is required."),
                new("Cpf", "CPF is required.")
            };

            _createUserValidatorMock
                .Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(validationErrors));

            // Act
            var result = await _userService.CreateUserAsync(dto);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error!.Code.Should().Be("User.Validation");
            result.Error.Message.Should().Contain("Name is required.");
            _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task CreateUserAsync_ValidData_ShouldReturnSuccessResult()
        {
            var dto = new CreateUserDto
            {
                Name = "John Doe",
                Cpf = "12345678909",
                BirthDate = new DateTime(1990, 1, 1),
                Role = UserRole.CUSTOMER
            };

            _userRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            var result = await _userService.CreateUserAsync(dto);

            result.IsSuccess.Should().BeTrue();
            result.Value!.Name.Should().Be(dto.Name);
            result.Value!.Cpf.Should().Be("123.456.789-09");
            _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task GetUserByIdAsync_ExistingId_ShouldReturnSuccessResult()
        {
            var user = User.Create("John Doe", "123.456.789-09", new DateTime(1990, 1, 1), UserRole.CUSTOMER);

            _userRepositoryMock
                .Setup(r => r.GetUserByIdAsync(user.Id))
                .ReturnsAsync(user);

            var result = await _userService.GetUserByIdAsync(user.Id);

            result.IsSuccess.Should().BeTrue();
            result.Value!.Id.Should().Be(user.Id);
            result.Value!.Name.Should().Be(user.Name);
        }

        [Fact]
        public async Task GetUserByIdAsync_NonExistingId_ShouldReturnNotFoundError()
        {
            var id = Guid.NewGuid();

            _userRepositoryMock
                .Setup(r => r.GetUserByIdAsync(id))
                .ReturnsAsync((User?)null);

            var result = await _userService.GetUserByIdAsync(id);

            result.IsSuccess.Should().BeFalse();
            result.Error!.Type.Should().Be(ErrorType.NotFound);
            result.Error!.Code.Should().Be("User.NotFound");
        }

        [Fact]
        public async Task UpdateUserAsync_WithInvalidDto_ReturnsValidationError()
        {
            // Arrange
            var id = Guid.NewGuid();
            var dto = new UpdateUserDto();

            var validationErrors = new List<ValidationFailure>
            {
                new("Name", "Name is required."),
                new("Cpf", "CPF is required.")
            };

            _updateUserValidatorMock
                .Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(validationErrors));

            // Act
            var result = await _userService.UpdateUserAsync(id, dto);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error!.Code.Should().Be("User.Validation");
            _userRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task UpdateUserAsync_ExistingId_ShouldReturnSuccessAndCallRepositoryUpdate()
        {
            var user = User.Create("John Doe", "123.456.789-09", new DateTime(1990, 1, 1), UserRole.CUSTOMER);

            var dto = new UpdateUserDto
            {
                Name = "John Smith",
                Cpf = "11122233344",
                BirthDate = new DateTime(1992, 8, 20),
                Role = UserRole.ADMIN
            };

            _userRepositoryMock
                .Setup(r => r.GetUserByIdAsync(user.Id))
                .ReturnsAsync(user);

            _userRepositoryMock
                .Setup(r => r.UpdateAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            var result = await _userService.UpdateUserAsync(user.Id, dto);

            result.IsSuccess.Should().BeTrue();
            _userRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task UpdateUserAsync_NonExistingId_ShouldReturnNotFoundError()
        {
            var id = Guid.NewGuid();

            _userRepositoryMock
                .Setup(r => r.GetUserByIdAsync(id))
                .ReturnsAsync((User?)null);

            var result = await _userService.UpdateUserAsync(id, new UpdateUserDto());

            result.IsSuccess.Should().BeFalse();
            result.Error!.Type.Should().Be(ErrorType.NotFound);
            result.Error!.Code.Should().Be("User.NotFound");
        }

        [Fact]
        public async Task DeleteUserAsync_ExistingId_ShouldReturnSuccessAndCallRepositoryDelete()
        {
            var user = User.Create("John Doe", "123.456.789-09", new DateTime(1990, 1, 1), UserRole.CUSTOMER);

            _userRepositoryMock
                .Setup(r => r.GetUserByIdAsync(user.Id))
                .ReturnsAsync(user);

            _userRepositoryMock
                .Setup(r => r.DeleteAsync(user.Id))
                .Returns(Task.CompletedTask);

            var result = await _userService.DeleteUserAsync(user.Id);

            result.IsSuccess.Should().BeTrue();
            _userRepositoryMock.Verify(r => r.DeleteAsync(user.Id), Times.Once);
        }

        [Fact]
        public async Task DeleteUserAsync_NonExistingId_ShouldReturnNotFoundError()
        {
            var id = Guid.NewGuid();

            _userRepositoryMock
                .Setup(r => r.GetUserByIdAsync(id))
                .ReturnsAsync((User?)null);

            var result = await _userService.DeleteUserAsync(id);

            result.IsSuccess.Should().BeFalse();
            result.Error!.Type.Should().Be(ErrorType.NotFound);
            result.Error!.Code.Should().Be("User.NotFound");
        }
    }
}
