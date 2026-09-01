using Fiap.VehicleSales.Domain.Common;
using Fiap.VehicleSales.Domain.Enums;
using Fiap.VehicleSales.Domain.Exceptions;

namespace Fiap.VehicleSales.Domain.Entities;

public sealed class Vehicle : Entity
{
    public string Brand { get; private set; }
    public string Model { get; private set; }
    public int Year { get; private set; }
    public string Color { get; private set; }
    public decimal Price { get; private set; }
    public VehicleStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private Vehicle()
    {
        Brand = string.Empty;
        Model = string.Empty;
        Color = string.Empty;
    }

    public Vehicle(string brand, string model, int year, string color, decimal price)
    {
        Validate(brand, model, year, color, price);

        Brand = brand.Trim();
        Model = model.Trim();
        Year = year;
        Color = color.Trim();
        Price = price;
        Status = VehicleStatus.Available;
        CreatedAt = DateTime.UtcNow;
    }

    public void Update(string brand, string model, int year, string color, decimal price)
    {
        if (Status == VehicleStatus.Sold)
            throw new DomainException("Não é possível editar um veículo vendido.");

        Validate(brand, model, year, color, price);

        Brand = brand.Trim();
        Model = model.Trim();
        Year = year;
        Color = color.Trim();
        Price = price;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsSold()
    {
        if (Status == VehicleStatus.Sold)
            throw new DomainException("Este veículo já foi vendido.");

        Status = VehicleStatus.Sold;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsAvailable()
    {
        return Status == VehicleStatus.Available;
    }

    private static void Validate(string brand, string model, int year, string color, decimal price)
    {
        if (string.IsNullOrWhiteSpace(brand))
            throw new DomainException("A marca do veículo é obrigatória.");

        if (string.IsNullOrWhiteSpace(model))
            throw new DomainException("O modelo do veículo é obrigatório.");

        if (year < 1900 || year > DateTime.UtcNow.Year + 1)
            throw new DomainException("O ano do veículo é inválido.");

        if (string.IsNullOrWhiteSpace(color))
            throw new DomainException("A cor do veículo é obrigatória.");

        if (price <= 0)
            throw new DomainException("O preço do veículo deve ser maior que zero.");
    }
}