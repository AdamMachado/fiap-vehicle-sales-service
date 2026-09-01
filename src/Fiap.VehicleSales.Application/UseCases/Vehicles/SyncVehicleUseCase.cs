using Fiap.VehicleSales.Application.DTOs;
using Fiap.VehicleSales.Application.Interfaces;
using Fiap.VehicleSales.Application.Mappers;
using Fiap.VehicleSales.Domain.Entities;

namespace Fiap.VehicleSales.Application.UseCases.Vehicles;

public sealed class SyncVehicleUseCase
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SyncVehicleUseCase(IVehicleRepository vehicleRepository, IUnitOfWork unitOfWork)
    {
        _vehicleRepository = vehicleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<SyncVehicleResult> ExecuteAsync(Guid id, SyncVehicleRequest request)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(id);
        var created = vehicle is null;

        if (created)
        {
            vehicle = new Vehicle(id, request.Brand, request.Model, request.Year, request.Color, request.Price);
            await _vehicleRepository.AddAsync(vehicle);
        }
        else
        {
            vehicle!.Update(request.Brand, request.Model, request.Year, request.Color, request.Price);
            await _vehicleRepository.UpdateAsync(vehicle);
        }

        await _unitOfWork.CommitAsync();
        return new SyncVehicleResult(VehicleMapper.ToResponse(vehicle), created);
    }
}
