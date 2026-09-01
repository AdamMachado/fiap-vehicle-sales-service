using Fiap.VehicleSales.Application.DTOs;
using Fiap.VehicleSales.Application.Exceptions;
using Fiap.VehicleSales.Application.Interfaces;
using Fiap.VehicleSales.Application.Mappers;
using Fiap.VehicleSales.Domain.Entities;

namespace Fiap.VehicleSales.Application.UseCases.Sales;

public sealed class ProcessPaymentUseCase
{
    private readonly ISaleRepository _saleRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProcessPaymentUseCase(
        ISaleRepository saleRepository,
        IVehicleRepository vehicleRepository,
        IUnitOfWork unitOfWork)
    {
        _saleRepository = saleRepository;
        _vehicleRepository = vehicleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<SaleResponse> ExecuteAsync(string paymentCode, ProcessPaymentRequest request)
    {
        if (string.IsNullOrWhiteSpace(paymentCode))
            throw new AppException("Código do pagamento é obrigatório.");

        var sale = await _saleRepository.GetByPaymentCodeAsync(paymentCode);
        if (sale is null)
            throw new AppException("Pagamento não encontrado.");

        var vehicle = await _vehicleRepository.GetByIdAsync(sale.VehicleId);
        if (vehicle is null)
            throw new AppException("Veículo da venda não encontrado.");

        var changed = request.Status.Trim().ToLowerInvariant() switch
        {
            "completed" or "paid" or "efetuado" => Complete(sale, vehicle),
            "canceled" or "cancelled" or "cancelado" => Cancel(sale, vehicle),
            _ => throw new AppException("Status de pagamento inválido. Use Completed ou Canceled.")
        };

        if (changed)
        {
            await _saleRepository.UpdateAsync(sale);
            await _vehicleRepository.UpdateAsync(vehicle);
            await _unitOfWork.CommitAsync();
        }

        return SaleMapper.ToResponse(sale);
    }

    private static bool Complete(Sale sale, Vehicle vehicle)
    {
        if (!sale.CompletePayment())
            return false;

        vehicle.MarkAsSold();
        return true;
    }

    private static bool Cancel(Sale sale, Vehicle vehicle)
    {
        if (!sale.CancelPayment())
            return false;

        vehicle.ReleaseReservation();
        return true;
    }
}
