using Fiap.VehicleSales.Application.DTOs;
using Fiap.VehicleSales.Application.Interfaces;
using Fiap.VehicleSales.Application.Mappers;
using Fiap.VehicleSales.Domain.Entities;

namespace Fiap.VehicleSales.Application.UseCases.Vehicles;

public sealed class CreateVehicleUseCase
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateVehicleUseCase(
        IVehicleRepository vehicleRepository,
        IUnitOfWork unitOfWork)
    {
        _vehicleRepository = vehicleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<VehicleResponse> ExecuteAsync(CreateVehicleRequest request)
    {
        var vehicle = new Vehicle(
            request.Brand,
            request.Model,
            request.Year,
            request.Color,
            request.Price
        );

        await _vehicleRepository.AddAsync(vehicle);
        await _unitOfWork.CommitAsync();

        return VehicleMapper.ToResponse(vehicle);
    }
}