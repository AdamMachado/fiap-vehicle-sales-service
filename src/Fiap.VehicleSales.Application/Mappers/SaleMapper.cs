using Fiap.VehicleSales.Application.DTOs;
using Fiap.VehicleSales.Domain.Entities;

namespace Fiap.VehicleSales.Application.Mappers;

public static class SaleMapper
{
    public static SaleResponse ToResponse(Sale sale)
    {
        return new SaleResponse
        {
            Id = sale.Id,
            VehicleId = sale.VehicleId,
            BuyerId = sale.BuyerId,
            BuyerCpf = sale.BuyerCpf,
            PaymentCode = sale.PaymentCode,
            Price = sale.Price,
            SaleDate = sale.SaleDate,
            PaymentProcessedAt = sale.PaymentProcessedAt,
            Status = sale.Status.ToString()
        };
    }
}
