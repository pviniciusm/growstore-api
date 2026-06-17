using FluentAssertions;
using GrowStore.Application.Users.DTOs;
using GrowStore.Application.Users.Services;
using GrowStore.Domain.Entities;
using GrowStore.Domain.Interfaces;
using GrowStore.Domain.Shared.Enums;
using Moq;

namespace Growstore.Tests.UnitTests.Services
{
    // Exemplo: Teste unitário para o UserService
    // - Mock<IUserRepository>: isola o serviço do banco de dados real
    // - UserService recebe o mock via construtor (Injeção de Dependência)
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
            // Arrange: prepara os dados de entrada e configura o comportamento do mock
            var dto = new CreateUserDto
            {
                Name = "John Doe",
                Cpf = "123.456.789-00",
                BirthDate = new DateTime(1990, 1, 1),
                Role = UserRole.CUSTOMER
            };

            // It.IsAny<User>(): aceita qualquer instância de User passada para o AddAsync
            _userRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            // Act: executa o método que está sendo testado
            var result = await _userService.CreateUserAsync(dto);

            // Assert: verifica se os dados retornados estão corretos
            result.Should().NotBeNull();
            result.Name.Should().Be(dto.Name);
            result.Cpf.Should().Be(dto.Cpf);

            // Verify: garante que o AddAsync foi chamado exatamente uma vez durante a execução
            _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
        }
    }
}
