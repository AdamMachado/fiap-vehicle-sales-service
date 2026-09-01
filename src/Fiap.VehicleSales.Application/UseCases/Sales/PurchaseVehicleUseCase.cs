using Fiap.VehicleSales.Application.DTOs;
using Fiap.VehicleSales.Application.Exceptions;
using Fiap.VehicleSales.Application.Interfaces;
using Fiap.VehicleSales.Application.Mappers;
using Fiap.VehicleSales.Domain.Entities;

namespace Fiap.VehicleSales.Application.UseCases.Sales;

public sealed class PurchaseVehicleUseCase
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly ISaleRepository _saleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PurchaseVehicleUseCase(
        IVehicleRepository vehicleRepository,
        ISaleRepository saleRepository,
        IUnitOfWork unitOfWork)
    {
        _vehicleRepository = vehicleRepository;
        _saleRepository = saleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<SaleResponse> ExecuteAsync(PurchaseVehicleRequest request, string buyerId)
    {
        if (string.IsNullOrWhiteSpace(buyerId))
            throw new AppException("Comprador não identificado.");

        var vehicle = await _vehicleRepository.GetByIdAsync(request.VehicleId);

        if (vehicle is null)
            throw new AppException("Veículo não encontrado.");

        if (!vehicle.IsAvailable())
            throw new AppException("Veículo não está disponível para venda.");

        var paymentCode = Guid.NewGuid().ToString("N");
        var sale = new Sale(vehicle.Id, buyerId, request.BuyerCpf, vehicle.Price, paymentCode);

        vehicle.Reserve();

        await _saleRepository.AddAsync(sale);
        await _vehicleRepository.UpdateAsync(vehicle);
        await _unitOfWork.CommitAsync();

        return SaleMapper.ToResponse(sale);
    }
}
