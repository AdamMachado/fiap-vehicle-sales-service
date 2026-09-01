using Fiap.VehicleSales.Application.DTOs;
using Fiap.VehicleSales.Application.Exceptions;
using Fiap.VehicleSales.Application.Interfaces;
using Fiap.VehicleSales.Application.Mappers;

namespace Fiap.VehicleSales.Application.UseCases.Vehicles;

public sealed class UpdateVehicleUseCase
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateVehicleUseCase(
        IVehicleRepository vehicleRepository,
        IUnitOfWork unitOfWork)
    {
        _vehicleRepository = vehicleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<VehicleResponse> ExecuteAsync(Guid id, UpdateVehicleRequest request)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(id);

        if (vehicle is null)
            throw new AppException("Veículo não encontrado.");

        vehicle.Update(
            request.Brand,
            request.Model,
            request.Year,
            request.Color,
            request.Price
        );

        await _vehicleRepository.UpdateAsync(vehicle);
        await _unitOfWork.CommitAsync();

        return VehicleMapper.ToResponse(vehicle);
    }
}