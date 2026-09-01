using Fiap.VehicleSales.Application.Interfaces;
using Fiap.VehicleSales.Domain.Entities;
using Fiap.VehicleSales.Domain.Enums;

namespace Fiap.VehicleSales.UnitTests.Fakes;

public sealed class FakeVehicleRepository : IVehicleRepository
{
    private readonly List<Vehicle> _vehicles = [];

    public Task AddAsync(Vehicle vehicle)
    {
        _vehicles.Add(vehicle);
        return Task.CompletedTask;
    }

    public Task<Vehicle?> GetByIdAsync(Guid id)
    {
        var vehicle = _vehicles.FirstOrDefault(vehicle => vehicle.Id == id);

        return Task.FromResult(vehicle);
    }

    public Task<IReadOnlyList<Vehicle>> GetAvailableAsync()
    {
        IReadOnlyList<Vehicle> vehicles = _vehicles
            .Where(vehicle => vehicle.Status == VehicleStatus.Available)
            .OrderBy(vehicle => vehicle.Price)
            .ToList();

        return Task.FromResult(vehicles);
    }

    public Task<IReadOnlyList<Vehicle>> GetSoldAsync()
    {
        IReadOnlyList<Vehicle> vehicles = _vehicles
            .Where(vehicle => vehicle.Status == VehicleStatus.Sold)
            .OrderBy(vehicle => vehicle.Price)
            .ToList();

        return Task.FromResult(vehicles);
    }

    public Task UpdateAsync(Vehicle vehicle)
    {
        var currentVehicle = _vehicles.FirstOrDefault(item => item.Id == vehicle.Id);

        if (currentVehicle is null)
            _vehicles.Add(vehicle);

        return Task.CompletedTask;
    }
}