using GrowStore.Application.Addresses.Services;
using GrowStore.Domain.Entities.Addresses;
using GrowStore.Infrastructure.Repositories;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Growstore.Tests.UnitTests.Services;

public class AddressServiceTests
{
    [Fact]
    public async Task GetByIdAsync_ShouldReturnAddress_WhenAddressExists()
    {
        // ARRANGE
        var repositoryMock = new AddressRepositoryMock();
        var service = new AddressService(repositoryMock); // Sem AutoMapper!
        
        var existingAddress = new Address(Guid.NewGuid(), "Rua Teste", "Brasília", "DF", "70000-000", true);
        await repositoryMock.AddAsync(existingAddress);

        // ACT
        var result = await service.GetByIdAsync(existingAddress.Id);

        // ASSERT
        Assert.NotNull(result);
        Assert.Equal(existingAddress.Id, result.Id);
    }
}