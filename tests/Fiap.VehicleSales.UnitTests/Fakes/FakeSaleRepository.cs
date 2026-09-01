using Fiap.VehicleSales.Application.Interfaces;
using Fiap.VehicleSales.Domain.Entities;

namespace Fiap.VehicleSales.UnitTests.Fakes;

public sealed class FakeSaleRepository : ISaleRepository
{
    private readonly List<Sale> _sales = [];

    public IReadOnlyList<Sale> Sales => _sales;

    public Task AddAsync(Sale sale)
    {
        _sales.Add(sale);
        return Task.CompletedTask;
    }

    public Task<Sale?> GetByIdAsync(Guid id)
    {
        var sale = _sales.FirstOrDefault(sale => sale.Id == id);

        return Task.FromResult(sale);
    }
}