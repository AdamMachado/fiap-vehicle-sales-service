using Fiap.VehicleSales.Domain.Entities;
using Fiap.VehicleSales.Domain.Enums;
using Fiap.VehicleSales.Domain.Exceptions;

namespace Fiap.VehicleSales.UnitTests.Domain.Entities;

public sealed class SaleTests
{
    [Fact]
    public void Constructor_ShouldCreateSale_WhenDataIsValid()
    {
        // Arrange
        var vehicleId = Guid.NewGuid();
        var buyerId = "buyer-test-001";
        var price = 120000m;

        // Act
        var sale = new Sale(vehicleId, buyerId, price);

        // Assert
        Assert.NotEqual(Guid.Empty, sale.Id);
        Assert.Equal(vehicleId, sale.VehicleId);
        Assert.Equal(buyerId, sale.BuyerId);
        Assert.Equal(price, sale.Price);
        Assert.Equal(SaleStatus.Completed, sale.Status);
        Assert.NotEqual(default, sale.SaleDate);
    }

    [Fact]
    public void Constructor_ShouldTrimBuyerId_WhenBuyerIdHasWhiteSpaces()
    {
        // Act
        var sale = new Sale(Guid.NewGuid(), " buyer-test-001 ", 120000m);

        // Assert
        Assert.Equal("buyer-test-001", sale.BuyerId);
    }

    [Fact]
    public void Constructor_ShouldThrowDomainException_WhenVehicleIdIsEmpty()
    {
        // Act
        var act = () => new Sale(Guid.Empty, "buyer-test-001", 120000m);

        // Assert
        Assert.Throws<DomainException>(act);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_ShouldThrowDomainException_WhenBuyerIdIsInvalid(string buyerId)
    {
        // Act
        var act = () => new Sale(Guid.NewGuid(), buyerId, 120000m);

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
        var act = () => new Sale(Guid.NewGuid(), "buyer-test-001", price);

        // Assert
        Assert.Throws<DomainException>(act);
    }

    [Fact]
    public void Cancel_ShouldChangeStatusToCanceled_WhenSaleIsCompleted()
    {
        // Arrange
        var sale = new Sale(Guid.NewGuid(), "buyer-test-001", 120000m);

        // Act
        sale.Cancel();

        // Assert
        Assert.Equal(SaleStatus.Canceled, sale.Status);
    }

    [Fact]
    public void Cancel_ShouldThrowDomainException_WhenSaleIsAlreadyCanceled()
    {
        // Arrange
        var sale = new Sale(Guid.NewGuid(), "buyer-test-001", 120000m);
        sale.Cancel();

        // Act
        var act = () => sale.Cancel();

        // Assert
        Assert.Throws<DomainException>(act);
    }
}