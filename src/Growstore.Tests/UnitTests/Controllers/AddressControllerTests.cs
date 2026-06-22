using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using GrowStore.API.Controllers;
using GrowStore.Infrastructure.Repositories;
using GrowStore.Domain.Entities.Addresses; 

namespace Growstore.Tests.UnitTests.Controllers;

public class AddressControllerTests
{
    [Fact]
public async Task GetById_ReturnsOkResult_WhenAddressExists()
{
    // 1. ARRANGE
    var repositoryMock = new AddressRepositoryMock();
    
    var newAddress = new Address(Guid.NewGuid(), "Exemplo", "Brasília", "DF", "00000-000", true);
    
    await repositoryMock.AddAsync(newAddress);

    var controller = new AddressController(repositoryMock);

   
    var result = await controller.GetById(newAddress.Id); 

    var okResult = Assert.IsType<OkObjectResult>(result); 
    var returnAddress = Assert.IsType<Address>(okResult.Value); 
    Assert.Equal(newAddress.Id, returnAddress.Id); 
}

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenAddressDoesNotExist()
    {
        var repositoryMock = new AddressRepositoryMock();
        var controller = new AddressController(repositoryMock);
        var nonExistentId = Guid.NewGuid(); 

        var result = await controller.GetById(nonExistentId);

        Assert.IsType<NotFoundObjectResult>(result); 
    } 

} 