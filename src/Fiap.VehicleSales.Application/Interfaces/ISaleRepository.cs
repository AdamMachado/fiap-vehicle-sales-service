using Fiap.VehicleSales.Domain.Entities;

namespace Fiap.VehicleSales.Application.Interfaces;

public interface ISaleRepository
{
    Task AddAsync(Sale sale);
    Task<Sale?> GetByIdAsync(Guid id);
}