namespace Fiap.VehicleSales.Application.DTOs;

public sealed class PurchaseVehicleRequest
{
    public Guid VehicleId { get; set; }
    public string BuyerCpf { get; set; } = string.Empty;
}
