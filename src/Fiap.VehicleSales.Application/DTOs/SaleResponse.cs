namespace Fiap.VehicleSales.Application.DTOs;

public sealed class SaleResponse
{
    public Guid Id { get; set; }
    public Guid VehicleId { get; set; }
    public string BuyerId { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime SaleDate { get; set; }
    public string Status { get; set; } = string.Empty;
}