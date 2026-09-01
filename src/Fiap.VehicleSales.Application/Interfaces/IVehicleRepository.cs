using Fiap.VehicleSales.Domain.Entities;

namespace Fiap.VehicleSales.Application.Interfaces;

public interface IVehicleRepository
{
    Task AddAsync(Vehicle vehicle);
    Task<Vehicle?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<Vehicle>> GetAvailableAsync();
    Task<IReadOnlyList<Vehicle>> GetSoldAsync();
    Task UpdateAsync(Vehicle vehicle);
}