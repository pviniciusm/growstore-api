using GrowStore.Domain.Entities.Addresses;
using Xunit;

namespace Growstore.Tests.UnitTests.Entities;

public class AddressTests
{
    [Fact]
    public void Constructor_ShouldCreateAddress_WhenDataIsValid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var street = "Rua das Flores";
        var city = "Brasília";
        var state = "DF";
        var zipCode = "70000-000";
        var isDefault = true;

        // Act
        var address = new Address(userId, street, city, state, zipCode, isDefault);

        // Assert
        Assert.NotEqual(Guid.Empty, address.Id); // Garante que o ID foi gerado
        Assert.Equal(street, address.Street);
        Assert.Equal(city, address.City);
        Assert.Equal(state, address.State);
    }
}