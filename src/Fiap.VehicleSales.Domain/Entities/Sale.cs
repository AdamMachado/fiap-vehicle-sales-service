using Fiap.VehicleSales.Domain.Common;
using Fiap.VehicleSales.Domain.Enums;
using Fiap.VehicleSales.Domain.Exceptions;

namespace Fiap.VehicleSales.Domain.Entities;

public sealed class Sale : Entity
{
    public Guid VehicleId { get; private set; }
    public string BuyerId { get; private set; }
    public decimal Price { get; private set; }
    public DateTime SaleDate { get; private set; }
    public SaleStatus Status { get; private set; }

    private Sale()
    {
        BuyerId = string.Empty;
    }

    public Sale(Guid vehicleId, string buyerId, decimal price)
    {
        if (vehicleId == Guid.Empty)
            throw new DomainException("O veículo da venda é obrigatório.");

        if (string.IsNullOrWhiteSpace(buyerId))
            throw new DomainException("O comprador da venda é obrigatório.");

        if (price <= 0)
            throw new DomainException("O preço da venda deve ser maior que zero.");

        VehicleId = vehicleId;
        BuyerId = buyerId.Trim();
        Price = price;
        SaleDate = DateTime.UtcNow;
        Status = SaleStatus.Completed;
    }

    public void Cancel()
    {
        if (Status == SaleStatus.Canceled)
            throw new DomainException("Esta venda já foi cancelada.");

        Status = SaleStatus.Canceled;
    }
}