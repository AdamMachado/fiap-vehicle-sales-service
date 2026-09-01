using Fiap.VehicleSales.Application.DTOs;
using Fiap.VehicleSales.Application.UseCases.Vehicles;
using Fiap.VehicleSales.Domain.Entities;
using Fiap.VehicleSales.Domain.Exceptions;
using Fiap.VehicleSales.UnitTests.Fakes;

namespace Fiap.VehicleSales.UnitTests.Application.UseCases.Vehicles;

public sealed class SyncVehicleUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_ShouldCreateVehicleWithExternalId_WhenItDoesNotExist()
    {
        var repository = new FakeVehicleRepository();
        var unitOfWork = new FakeUnitOfWork();
        var useCase = new SyncVehicleUseCase(repository, unitOfWork);
        var id = Guid.NewGuid();

        var result = await useCase.ExecuteAsync(id, Request("Corolla", 120000m));

        Assert.True(result.Created);
        Assert.Equal(id, result.Vehicle.Id);
        Assert.NotNull(await repository.GetByIdAsync(id));
        Assert.True(unitOfWork.WasCommitted);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldUpdateVehicle_WhenItAlreadyExists()
    {
        var repository = new FakeVehicleRepository();
        var unitOfWork = new FakeUnitOfWork();
        var vehicle = new Vehicle(Guid.NewGuid(), "Toyota", "Corolla", 2022, "Prata", 120000m);
        await repository.AddAsync(vehicle);
        var useCase = new SyncVehicleUseCase(repository, unitOfWork);

        var result = await useCase.ExecuteAsync(vehicle.Id, Request("Corolla XEi", 118000m));

        Assert.False(result.Created);
        Assert.Equal("Corolla XEi", result.Vehicle.Model);
        Assert.Equal(118000m, result.Vehicle.Price);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldRejectUpdate_WhenVehicleIsReserved()
    {
        var repository = new FakeVehicleRepository();
        var vehicle = new Vehicle(Guid.NewGuid(), "Toyota", "Corolla", 2022, "Prata", 120000m);
        vehicle.Reserve();
        await repository.AddAsync(vehicle);
        var useCase = new SyncVehicleUseCase(repository, new FakeUnitOfWork());

        var act = () => useCase.ExecuteAsync(vehicle.Id, Request("Corolla XEi", 118000m));

        await Assert.ThrowsAsync<DomainException>(act);
    }

    private static SyncVehicleRequest Request(string model, decimal price) => new()
    {
        Brand = "Toyota",
        Model = model,
        Year = 2022,
        Color = "Prata",
        Price = price
    };
}
