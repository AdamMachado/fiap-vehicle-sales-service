using Fiap.VehicleSales.Application.DTOs;
using Fiap.VehicleSales.Application.Interfaces;
using Fiap.VehicleSales.Domain.Entities;

namespace Fiap.VehicleSales.Application.UseCases.Vehicles;

public sealed class ListSoldVehiclesUseCase
{
    private readonly IVehicleRepository _vehicleRepository;

    public ListSoldVehiclesUseCase(IVehicleRepository vehicleRepository)
    {
        _vehicleRepository = vehicleRepository;
    }

    public async Task<IReadOnlyList<VehicleResponse>> ExecuteAsync()
    {
        var vehicles = await _vehicleRepository.GetSoldAsync();

        return vehicles.Select(ToResponse).ToList();
    }

    private static VehicleResponse ToResponse(Vehicle vehicle)
    {
        return new VehicleResponse
        {
            Id = vehicle.Id,
            Brand = vehicle.Brand,
            Model = vehicle.Model,
            Year = vehicle.Year,
            Color = vehicle.Color,
            Price = vehicle.Price,
            Status = vehicle.Status.ToString()
        };
    }
}