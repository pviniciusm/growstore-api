using FluentAssertions;

namespace Growstore.Tests.UnitTests.Entities
{
    // Exemplo: Teste unitário para a entidade User
    // - Não há dependências externas, pois entidades são testadas de forma totalmente isolada
    public class UserTests
    {
        [Fact]
        public void Create_ValidData_ShouldCreateUserSuccessfully()
        {
            // Arrange: prepara os dados de entrada
            var name = "John Doe";
            var cpf = "123.456.789-00";
            var birthDate = new DateTime(1990, 1, 1);
            var role = UserRole.Customer;

            // Act: executa o factory method que cria o usuário
            var user = User.Create(name, cpf, birthDate, role);

            // Assert: verifica se a entidade foi criada com os valores corretos
            user.Id.Should().NotBeEmpty();       // Id deve ser gerado automaticamente
            user.Name.Should().Be(name);
            user.Cpf.Should().Be(cpf);
            user.BirthDate.Should().Be(birthDate);
            user.Role.Should().Be(role);
            user.IsActive.Should().BeTrue();     // usuário começa ativo por padrão
        }
    }
}
