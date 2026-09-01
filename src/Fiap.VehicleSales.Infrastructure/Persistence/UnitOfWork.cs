using Fiap.VehicleSales.Application.Interfaces;

namespace Fiap.VehicleSales.Infrastructure.Persistence;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly VehicleSalesDbContext _context;

    public UnitOfWork(VehicleSalesDbContext context)
    {
        _context = context;
    }

    public async Task CommitAsync()
    {
        await _context.SaveChangesAsync();
    }
}