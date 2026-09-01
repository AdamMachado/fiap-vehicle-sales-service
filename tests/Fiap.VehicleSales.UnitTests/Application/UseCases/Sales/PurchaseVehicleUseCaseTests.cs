using Fiap.VehicleSales.Application.DTOs;
using Fiap.VehicleSales.Application.Exceptions;
using Fiap.VehicleSales.Application.UseCases.Sales;
using Fiap.VehicleSales.Domain.Entities;
using Fiap.VehicleSales.Domain.Enums;
using Fiap.VehicleSales.UnitTests.Fakes;

namespace Fiap.VehicleSales.UnitTests.Application.UseCases.Sales;

public sealed class PurchaseVehicleUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_ShouldPurchaseVehicle_WhenVehicleIsAvailable()
    {
        // Arrange
        var vehicleRepository = new FakeVehicleRepository();
        var saleRepository = new FakeSaleRepository();
        var unitOfWork = new FakeUnitOfWork();

        var vehicle = new Vehicle("Toyota", "Corolla", 2022, "Prata", 120000m);
        await vehicleRepository.AddAsync(vehicle);

        var useCase = new PurchaseVehicleUseCase(
            vehicleRepository,
            saleRepository,
            unitOfWork
        );

        var request = new PurchaseVehicleRequest
        {
            VehicleId = vehicle.Id
        };

        var buyerId = "buyer-test-001";

        // Act
        var response = await useCase.ExecuteAsync(request, buyerId);

        // Assert
        Assert.NotEqual(Guid.Empty, response.Id);
        Assert.Equal(vehicle.Id, response.VehicleId);
        Assert.Equal(buyerId, response.BuyerId);
        Assert.Equal(120000m, response.Price);
        Assert.Equal("Completed", response.Status);

        Assert.Equal(VehicleStatus.Sold, vehicle.Status);
        Assert.False(vehicle.IsAvailable());

        Assert.Single(saleRepository.Sales);
        Assert.True(unitOfWork.WasCommitted);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowAppException_WhenBuyerIdIsEmpty()
    {
        // Arrange
        var vehicleRepository = new FakeVehicleRepository();
        var saleRepository = new FakeSaleRepository();
        var unitOfWork = new FakeUnitOfWork();

        var useCase = new PurchaseVehicleUseCase(
            vehicleRepository,
            saleRepository,
            unitOfWork
        );

        var request = new PurchaseVehicleRequest
        {
            VehicleId = Guid.NewGuid()
        };

        // Act
        var act = async () => await useCase.ExecuteAsync(request, "");

        // Assert
        await Assert.ThrowsAsync<AppException>(act);
        Assert.Empty(saleRepository.Sales);
        Assert.False(unitOfWork.WasCommitted);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowAppException_WhenVehicleDoesNotExist()
    {
        // Arrange
        var vehicleRepository = new FakeVehicleRepository();
        var saleRepository = new FakeSaleRepository();
        var unitOfWork = new FakeUnitOfWork();

        var useCase = new PurchaseVehicleUseCase(
            vehicleRepository,
            saleRepository,
            unitOfWork
        );

        var request = new PurchaseVehicleRequest
        {
            VehicleId = Guid.NewGuid()
        };

        // Act
        var act = async () => await useCase.ExecuteAsync(request, "buyer-test-001");

        // Assert
        await Assert.ThrowsAsync<AppException>(act);
        Assert.Empty(saleRepository.Sales);
        Assert.False(unitOfWork.WasCommitted);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowAppException_WhenVehicleIsAlreadySold()
    {
        // Arrange
        var vehicleRepository = new FakeVehicleRepository();
        var saleRepository = new FakeSaleRepository();
        var unitOfWork = new FakeUnitOfWork();

        var vehicle = new Vehicle("Toyota", "Corolla", 2022, "Prata", 120000m);
        vehicle.MarkAsSold();

        await vehicleRepository.AddAsync(vehicle);

        var useCase = new PurchaseVehicleUseCase(
            vehicleRepository,
            saleRepository,
            unitOfWork
        );

        var request = new PurchaseVehicleRequest
        {
            VehicleId = vehicle.Id
        };

        // Act
        var act = async () => await useCase.ExecuteAsync(request, "buyer-test-001");

        // Assert
        await Assert.ThrowsAsync<AppException>(act);
        Assert.Empty(saleRepository.Sales);
        Assert.False(unitOfWork.WasCommitted);
    }
}