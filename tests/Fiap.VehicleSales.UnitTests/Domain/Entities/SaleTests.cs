using Fiap.VehicleSales.Domain.Entities;
using Fiap.VehicleSales.Domain.Enums;
using Fiap.VehicleSales.Domain.Exceptions;

namespace Fiap.VehicleSales.UnitTests.Domain.Entities;

public sealed class SaleTests
{
    private const string ValidCpf = "52998224725";

    [Fact]
    public void Constructor_ShouldCreatePendingSale_WhenDataIsValid()
    {
        var vehicleId = Guid.NewGuid();
        var sale = new Sale(vehicleId, "buyer-test-001", ValidCpf, 120000m, "payment-001");

        Assert.NotEqual(Guid.Empty, sale.Id);
        Assert.Equal(vehicleId, sale.VehicleId);
        Assert.Equal("buyer-test-001", sale.BuyerId);
        Assert.Equal(ValidCpf, sale.BuyerCpf);
        Assert.Equal("payment-001", sale.PaymentCode);
        Assert.Equal(120000m, sale.Price);
        Assert.Equal(SaleStatus.Pending, sale.Status);
        Assert.NotEqual(default, sale.SaleDate);
        Assert.Null(sale.PaymentProcessedAt);
    }

    [Fact]
    public void Constructor_ShouldNormalizeCpfAndTrimIdentifiers()
    {
        var sale = new Sale(Guid.NewGuid(), " buyer-test-001 ", "529.982.247-25", 120000m, " payment-001 ");

        Assert.Equal("buyer-test-001", sale.BuyerId);
        Assert.Equal(ValidCpf, sale.BuyerCpf);
        Assert.Equal("payment-001", sale.PaymentCode);
    }

    [Theory]
    [InlineData("")]
    [InlineData("11111111111")]
    [InlineData("12345678900")]
    public void Constructor_ShouldThrowDomainException_WhenCpfIsInvalid(string cpf)
    {
        var act = () => new Sale(Guid.NewGuid(), "buyer-test-001", cpf, 120000m, "payment-001");
        Assert.Throws<DomainException>(act);
    }

    [Fact]
    public void Constructor_ShouldRejectEmptyVehicleId()
    {
        var act = () => new Sale(Guid.Empty, "buyer-test-001", ValidCpf, 120000m, "payment-001");
        Assert.Throws<DomainException>(act);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_ShouldRejectEmptyBuyerId(string buyerId)
    {
        var act = () => new Sale(Guid.NewGuid(), buyerId, ValidCpf, 120000m, "payment-001");
        Assert.Throws<DomainException>(act);
    }

    [Fact]
    public void Constructor_ShouldRejectEmptyPaymentCode()
    {
        var act = () => new Sale(Guid.NewGuid(), "buyer-test-001", ValidCpf, 120000m, " ");
        Assert.Throws<DomainException>(act);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_ShouldRejectInvalidPrice(decimal price)
    {
        var act = () => new Sale(Guid.NewGuid(), "buyer-test-001", ValidCpf, price, "payment-001");
        Assert.Throws<DomainException>(act);
    }

    [Fact]
    public void CompletePayment_ShouldBeIdempotent()
    {
        var sale = CreateSale();

        Assert.True(sale.CompletePayment());
        var processedAt = sale.PaymentProcessedAt;
        Assert.False(sale.CompletePayment());

        Assert.Equal(SaleStatus.Completed, sale.Status);
        Assert.Equal(processedAt, sale.PaymentProcessedAt);
    }

    [Fact]
    public void CancelPayment_ShouldBeIdempotent()
    {
        var sale = CreateSale();

        Assert.True(sale.CancelPayment());
        var processedAt = sale.PaymentProcessedAt;
        Assert.False(sale.CancelPayment());

        Assert.Equal(SaleStatus.Canceled, sale.Status);
        Assert.Equal(processedAt, sale.PaymentProcessedAt);
    }

    [Fact]
    public void CompletePayment_ShouldRejectCanceledSale()
    {
        var sale = CreateSale();
        sale.CancelPayment();

        Assert.Throws<DomainException>(() => sale.CompletePayment());
    }

    [Fact]
    public void CancelPayment_ShouldRejectCompletedSale()
    {
        var sale = CreateSale();
        sale.CompletePayment();

        Assert.Throws<DomainException>(() => sale.CancelPayment());
    }

    private static Sale CreateSale() =>
        new(Guid.NewGuid(), "buyer-test-001", ValidCpf, 120000m, "payment-001");
}
