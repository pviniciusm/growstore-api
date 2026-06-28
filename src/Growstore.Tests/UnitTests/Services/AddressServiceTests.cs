using FluentAssertions;
using GrowStore.Application.Addresses.DTOs;
using GrowStore.Application.Addresses.Services;
using GrowStore.Domain.Entities.Addresses;
using GrowStore.Domain.Interfaces;
using Moq;

namespace Growstore.Tests.UnitTests.Services
{
    public class AddressServiceTests
    {
        private readonly Mock<IAddressRepository> _addressRepositoryMock;
        private readonly AddressService _addressService;

        public AddressServiceTests()
        {
            _addressRepositoryMock = new Mock<IAddressRepository>();
            _addressService = new AddressService(_addressRepositoryMock.Object);
        }

        [Fact]
        public async Task CreateAsync_ValidData_ShouldReturnSuccess()
        {
            var dto = new CreateAddressDto
            {
                UserId = Guid.NewGuid(),
                Street = "Rua A",
                City = "São Paulo",
                State = "SP",
                ZipCode = "01000-000",
                IsDefault = true
            };

            _addressRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Address>()))
                .Returns(Task.CompletedTask);

            var result = await _addressService.CreateAsync(dto);

            result.IsSuccess.Should().BeTrue();
            result.Value!.Street.Should().Be(dto.Street);
            _addressRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Address>()), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_NonExistingId_ShouldReturnNotFound()
        {
            var id = Guid.NewGuid();

            _addressRepositoryMock
                .Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync((Address?)null);

            var result = await _addressService.GetByIdAsync(id);

            result.IsFailure.Should().BeTrue();
            result.Error!.Code.Should().Be("Address.NotFound");
        }

        [Fact]
        public async Task UpdateAsync_ExistingId_ShouldCallRepositoryUpdate()
        {
            var address = new Address(Guid.NewGuid(), "Rua A", "São Paulo", "SP", "01000-000", false);
            var dto = new UpdateAddressDto
            {
                Street = "Rua B",
                City = "Campinas",
                State = "SP",
                ZipCode = "13000-000",
                IsDefault = true
            };

            _addressRepositoryMock.Setup(r => r.GetByIdAsync(address.Id)).ReturnsAsync(address);
            _addressRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Address>())).Returns(Task.CompletedTask);

            var result = await _addressService.UpdateAsync(address.Id, dto);

            result.IsSuccess.Should().BeTrue();
            result.Value!.Street.Should().Be(dto.Street);
            _addressRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Address>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_NonExistingId_ShouldReturnNotFound()
        {
            var id = Guid.NewGuid();

            _addressRepositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Address?)null);

            var result = await _addressService.DeleteAsync(id);

            result.IsFailure.Should().BeTrue();
            result.Error!.Code.Should().Be("Address.NotFound");
        }
    }
}