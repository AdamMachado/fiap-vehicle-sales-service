using Fiap.VehicleSales.Application.DTOs;
using Fiap.VehicleSales.Application.Exceptions;
using Fiap.VehicleSales.Application.Interfaces;
using Fiap.VehicleSales.Domain.Entities;

namespace Fiap.VehicleSales.Application.UseCases.Vehicles;

public sealed class GetVehicleByIdUseCase
{
    private readonly IVehicleRepository _vehicleRepository;

    public GetVehicleByIdUseCase(IVehicleRepository vehicleRepository)
    {
        _vehicleRepository = vehicleRepository;
    }

    public async Task<VehicleResponse> ExecuteAsync(Guid id)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(id);

        if (vehicle is null)
            throw new AppException("Veículo não encontrado.");

        return ToResponse(vehicle);
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