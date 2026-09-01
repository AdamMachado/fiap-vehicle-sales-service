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
            Price = sale.Price,
            SaleDate = sale.SaleDate,
            Status = sale.Status.ToString()
        };
    }
}