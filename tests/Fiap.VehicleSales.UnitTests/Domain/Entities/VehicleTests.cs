using Fiap.VehicleSales.Domain.Entities;
using Fiap.VehicleSales.Domain.Enums;
using Fiap.VehicleSales.Domain.Exceptions;

namespace Fiap.VehicleSales.UnitTests.Domain.Entities;

public sealed class VehicleTests
{
    [Fact]
    public void Constructor_ShouldCreateVehicle_WhenDataIsValid()
    {
        // Arrange
        var brand = "Toyota";
        var model = "Corolla";
        var year = 2022;
        var color = "Prata";
        var price = 120000m;

        // Act
        var vehicle = new Vehicle(brand, model, year, color, price);

        // Assert
        Assert.NotEqual(Guid.Empty, vehicle.Id);
        Assert.Equal(brand, vehicle.Brand);
        Assert.Equal(model, vehicle.Model);
        Assert.Equal(year, vehicle.Year);
        Assert.Equal(color, vehicle.Color);
        Assert.Equal(price, vehicle.Price);
        Assert.Equal(VehicleStatus.Available, vehicle.Status);
        Assert.True(vehicle.IsAvailable());
        Assert.NotEqual(default, vehicle.CreatedAt);
        Assert.Null(vehicle.UpdatedAt);
    }

    [Fact]
    public void Constructor_ShouldUseExternalId_WhenProvided()
    {
        var id = Guid.NewGuid();

        var vehicle = new Vehicle(id, "Toyota", "Corolla", 2022, "Prata", 120000m);

        Assert.Equal(id, vehicle.Id);
    }

    [Fact]
    public void Constructor_ShouldRejectEmptyExternalId()
    {
        var act = () => new Vehicle(Guid.Empty, "Toyota", "Corolla", 2022, "Prata", 120000m);
        Assert.Throws<DomainException>(act);
    }

    [Fact]
    public void Constructor_ShouldTrimTextFields_WhenDataHasWhiteSpaces()
    {
        // Act
        var vehicle = new Vehicle(" Toyota ", " Corolla ", 2022, " Prata ", 120000m);

        // Assert
        Assert.Equal("Toyota", vehicle.Brand);
        Assert.Equal("Corolla", vehicle.Model);
        Assert.Equal("Prata", vehicle.Color);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_ShouldThrowDomainException_WhenBrandIsInvalid(string brand)
    {
        // Act
        var act = () => new Vehicle(brand, "Corolla", 2022, "Prata", 120000m);

        // Assert
        Assert.Throws<DomainException>(act);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_ShouldThrowDomainException_WhenModelIsInvalid(string model)
    {
        // Act
        var act = () => new Vehicle("Toyota", model, 2022, "Prata", 120000m);

        // Assert
        Assert.Throws<DomainException>(act);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_ShouldThrowDomainException_WhenColorIsInvalid(string color)
    {
        // Act
        var act = () => new Vehicle("Toyota", "Corolla", 2022, color, 120000m);

        // Assert
        Assert.Throws<DomainException>(act);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-1000)]
    public void Constructor_ShouldThrowDomainException_WhenPriceIsInvalid(decimal price)
    {
        // Act
        var act = () => new Vehicle("Toyota", "Corolla", 2022, "Prata", price);

        // Assert
        Assert.Throws<DomainException>(act);
    }

    [Theory]
    [InlineData(1899)]
    [InlineData(1800)]
    public void Constructor_ShouldThrowDomainException_WhenYearIsTooOld(int year)
    {
        // Act
        var act = () => new Vehicle("Toyota", "Corolla", year, "Prata", 120000m);

        // Assert
        Assert.Throws<DomainException>(act);
    }

    [Fact]
    public void Constructor_ShouldThrowDomainException_WhenYearIsTooFarInFuture()
    {
        // Arrange
        var invalidYear = DateTime.UtcNow.Year + 2;

        // Act
        var act = () => new Vehicle("Toyota", "Corolla", invalidYear, "Prata", 120000m);

        // Assert
        Assert.Throws<DomainException>(act);
    }

    [Fact]
    public void Update_ShouldUpdateVehicle_WhenDataIsValid()
    {
        // Arrange
        var vehicle = new Vehicle("Toyota", "Corolla", 2022, "Prata", 120000m);

        // Act
        vehicle.Update("Honda", "Civic", 2021, "Preto", 115000m);

        // Assert
        Assert.Equal("Honda", vehicle.Brand);
        Assert.Equal("Civic", vehicle.Model);
        Assert.Equal(2021, vehicle.Year);
        Assert.Equal("Preto", vehicle.Color);
        Assert.Equal(115000m, vehicle.Price);
        Assert.NotNull(vehicle.UpdatedAt);
    }

    [Fact]
    public void MarkAsSold_ShouldChangeStatusToSold_WhenVehicleIsAvailable()
    {
        // Arrange
        var vehicle = new Vehicle("Toyota", "Corolla", 2022, "Prata", 120000m);

        // Act
        vehicle.Reserve();
        vehicle.MarkAsSold();

        // Assert
        Assert.Equal(VehicleStatus.Sold, vehicle.Status);
        Assert.False(vehicle.IsAvailable());
        Assert.NotNull(vehicle.UpdatedAt);
    }

    [Fact]
    public void MarkAsSold_ShouldThrowDomainException_WhenVehicleIsAlreadySold()
    {
        // Arrange
        var vehicle = new Vehicle("Toyota", "Corolla", 2022, "Prata", 120000m);
        vehicle.Reserve();
        vehicle.MarkAsSold();

        // Act
        var act = () => vehicle.MarkAsSold();

        // Assert
        Assert.Throws<DomainException>(act);
    }

    [Fact]
    public void Update_ShouldThrowDomainException_WhenVehicleIsSold()
    {
        // Arrange
        var vehicle = new Vehicle("Toyota", "Corolla", 2022, "Prata", 120000m);
        vehicle.Reserve();
        vehicle.MarkAsSold();

        // Act
        var act = () => vehicle.Update("Honda", "Civic", 2021, "Preto", 115000m);

        // Assert
        Assert.Throws<DomainException>(act);
    }

    [Fact]
    public void ReserveAndReleaseReservation_ShouldRestoreAvailability()
    {
        var vehicle = new Vehicle("Toyota", "Corolla", 2022, "Prata", 120000m);

        vehicle.Reserve();
        Assert.Equal(VehicleStatus.Reserved, vehicle.Status);
        Assert.False(vehicle.IsAvailable());

        vehicle.ReleaseReservation();
        Assert.Equal(VehicleStatus.Available, vehicle.Status);
        Assert.True(vehicle.IsAvailable());
    }

    [Fact]
    public void Update_ShouldThrowDomainException_WhenVehicleIsReserved()
    {
        var vehicle = new Vehicle("Toyota", "Corolla", 2022, "Prata", 120000m);
        vehicle.Reserve();

        var act = () => vehicle.Update("Honda", "Civic", 2021, "Preto", 115000m);

        Assert.Throws<DomainException>(act);
    }
}
