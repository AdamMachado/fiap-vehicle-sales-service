using Fiap.VehicleSales.Application.DTOs;
using Fiap.VehicleSales.Application.Exceptions;
using Fiap.VehicleSales.Application.UseCases.Sales;
using Fiap.VehicleSales.Domain.Entities;
using Fiap.VehicleSales.Domain.Enums;
using Fiap.VehicleSales.Domain.Exceptions;
using Fiap.VehicleSales.UnitTests.Fakes;

namespace Fiap.VehicleSales.UnitTests.Application.UseCases.Sales;

public sealed class ProcessPaymentUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_ShouldCompleteSaleAndSellVehicle()
    {
        var context = await CreateContextAsync();

        var response = await context.UseCase.ExecuteAsync(
            context.Sale.PaymentCode,
            new ProcessPaymentRequest { Status = "Completed" });

        Assert.Equal("Completed", response.Status);
        Assert.NotNull(response.PaymentProcessedAt);
        Assert.Equal(SaleStatus.Completed, context.Sale.Status);
        Assert.Equal(VehicleStatus.Sold, context.Vehicle.Status);
        Assert.True(context.UnitOfWork.WasCommitted);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldCancelSaleAndReleaseVehicle()
    {
        var context = await CreateContextAsync();

        var response = await context.UseCase.ExecuteAsync(
            context.Sale.PaymentCode,
            new ProcessPaymentRequest { Status = "Canceled" });

        Assert.Equal("Canceled", response.Status);
        Assert.Equal(SaleStatus.Canceled, context.Sale.Status);
        Assert.Equal(VehicleStatus.Available, context.Vehicle.Status);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldNotCommitAgain_WhenNotificationIsRepeated()
    {
        var context = await CreateContextAsync();
        var request = new ProcessPaymentRequest { Status = "Completed" };

        await context.UseCase.ExecuteAsync(context.Sale.PaymentCode, request);
        context.UnitOfWork.Reset();
        await context.UseCase.ExecuteAsync(context.Sale.PaymentCode, request);

        Assert.False(context.UnitOfWork.WasCommitted);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldRejectConflictingNotification()
    {
        var context = await CreateContextAsync();
        await context.UseCase.ExecuteAsync(
            context.Sale.PaymentCode,
            new ProcessPaymentRequest { Status = "Completed" });

        var act = () => context.UseCase.ExecuteAsync(
            context.Sale.PaymentCode,
            new ProcessPaymentRequest { Status = "Canceled" });

        await Assert.ThrowsAsync<DomainException>(act);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldRejectUnknownPaymentCode()
    {
        var useCase = new ProcessPaymentUseCase(
            new FakeSaleRepository(),
            new FakeVehicleRepository(),
            new FakeUnitOfWork());

        var act = () => useCase.ExecuteAsync(
            "unknown",
            new ProcessPaymentRequest { Status = "Completed" });

        await Assert.ThrowsAsync<AppException>(act);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldRejectInvalidStatus()
    {
        var context = await CreateContextAsync();

        var act = () => context.UseCase.ExecuteAsync(
            context.Sale.PaymentCode,
            new ProcessPaymentRequest { Status = "Unknown" });

        await Assert.ThrowsAsync<AppException>(act);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldRejectEmptyPaymentCode()
    {
        var context = await CreateContextAsync();

        var act = () => context.UseCase.ExecuteAsync(
            " ",
            new ProcessPaymentRequest { Status = "Completed" });

        await Assert.ThrowsAsync<AppException>(act);
    }

    private static async Task<TestContext> CreateContextAsync()
    {
        var vehicleRepository = new FakeVehicleRepository();
        var saleRepository = new FakeSaleRepository();
        var unitOfWork = new FakeUnitOfWork();
        var vehicle = new Vehicle("Toyota", "Corolla", 2022, "Prata", 120000m);
        vehicle.Reserve();
        var sale = new Sale(vehicle.Id, "buyer-001", "52998224725", vehicle.Price, "payment-001");

        await vehicleRepository.AddAsync(vehicle);
        await saleRepository.AddAsync(sale);

        return new TestContext(
            new ProcessPaymentUseCase(saleRepository, vehicleRepository, unitOfWork),
            vehicle,
            sale,
            unitOfWork);
    }

    private sealed record TestContext(
        ProcessPaymentUseCase UseCase,
        Vehicle Vehicle,
        Sale Sale,
        FakeUnitOfWork UnitOfWork);
}
