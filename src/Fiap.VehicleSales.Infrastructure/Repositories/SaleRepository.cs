using Fiap.VehicleSales.Application.Interfaces;
using Fiap.VehicleSales.Domain.Entities;
using Fiap.VehicleSales.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Fiap.VehicleSales.Infrastructure.Repositories;

public sealed class SaleRepository : ISaleRepository
{
    private readonly VehicleSalesDbContext _context;

    public SaleRepository(VehicleSalesDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Sale sale)
    {
        await _context.Sales.AddAsync(sale);
    }

    public async Task<Sale?> GetByIdAsync(Guid id)
    {
        return await _context.Sales
            .FirstOrDefaultAsync(sale => sale.Id == id);
    }

    public async Task<Sale?> GetByPaymentCodeAsync(string paymentCode)
    {
        return await _context.Sales
            .FirstOrDefaultAsync(sale => sale.PaymentCode == paymentCode);
    }

    public Task UpdateAsync(Sale sale)
    {
        _context.Sales.Update(sale);
        return Task.CompletedTask;
    }
}
