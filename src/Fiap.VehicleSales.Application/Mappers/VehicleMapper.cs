using Fiap.VehicleSales.Application.DTOs;
using Fiap.VehicleSales.Domain.Entities;

namespace Fiap.VehicleSales.Application.Mappers;

public static class VehicleMapper
{
    public static VehicleResponse ToResponse(Vehicle vehicle)
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