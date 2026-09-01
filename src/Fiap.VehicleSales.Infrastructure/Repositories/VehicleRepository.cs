using Fiap.VehicleSales.Application.Interfaces;
using Fiap.VehicleSales.Domain.Entities;
using Fiap.VehicleSales.Domain.Enums;
using Fiap.VehicleSales.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Fiap.VehicleSales.Infrastructure.Repositories;

public sealed class VehicleRepository : IVehicleRepository
{
    private readonly VehicleSalesDbContext _context;

    public VehicleRepository(VehicleSalesDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Vehicle vehicle)
    {
        await _context.Vehicles.AddAsync(vehicle);
    }

    public async Task<Vehicle?> GetByIdAsync(Guid id)
    {
        return await _context.Vehicles
            .FirstOrDefaultAsync(vehicle => vehicle.Id == id);
    }

    public async Task<IReadOnlyList<Vehicle>> GetAvailableAsync()
    {
        return await _context.Vehicles
            .Where(vehicle => vehicle.Status == VehicleStatus.Available)
            .OrderBy(vehicle => vehicle.Price)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Vehicle>> GetSoldAsync()
    {
        return await _context.Vehicles
            .Where(vehicle => vehicle.Status == VehicleStatus.Sold)
            .OrderBy(vehicle => vehicle.Price)
            .ToListAsync();
    }

    public Task UpdateAsync(Vehicle vehicle)
    {
        _context.Vehicles.Update(vehicle);
        return Task.CompletedTask;
    }
}