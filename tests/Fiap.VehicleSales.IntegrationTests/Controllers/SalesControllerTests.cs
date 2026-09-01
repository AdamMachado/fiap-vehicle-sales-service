using System.Net;
using System.Net.Http.Json;
using Fiap.VehicleSales.Application.DTOs;
using Fiap.VehicleSales.IntegrationTests.Factory;
using FluentAssertions;

namespace Fiap.VehicleSales.IntegrationTests.Controllers;

public sealed class SalesControllerTests
{
    [Fact]
    public async Task Purchase_ShouldReturnOk_WhenUserIsBuyerAndVehicleIsAvailable()
    {
        // Arrange
        await using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();

        client.DefaultRequestHeaders.Add("X-Test-User", "admin-test-001");
        client.DefaultRequestHeaders.Add("X-Test-Role", "admin");

        var createVehicleResponse = await client.PostAsJsonAsync("/api/vehicles", new CreateVehicleRequest
        {
            Brand = "Toyota",
            Model = "Corolla",
            Year = 2022,
            Color = "Prata",
            Price = 120000m
        });

        var vehicle = await createVehicleResponse.Content.ReadFromJsonAsync<VehicleResponse>();

        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("X-Test-User", "buyer-test-001");
        client.DefaultRequestHeaders.Add("X-Test-Role", "buyer");

        var purchaseRequest = new PurchaseVehicleRequest
        {
            VehicleId = vehicle!.Id,
            BuyerCpf = "52998224725"
        };

        // Act
        var purchaseResponse = await client.PostAsJsonAsync("/api/sales", purchaseRequest);

        // Assert
        purchaseResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var sale = await purchaseResponse.Content.ReadFromJsonAsync<SaleResponse>();

        sale.Should().NotBeNull();
        sale!.Id.Should().NotBeEmpty();
        sale.VehicleId.Should().Be(vehicle.Id);
        sale.BuyerId.Should().Be("buyer-test-001");
        sale.BuyerCpf.Should().Be("52998224725");
        sale.PaymentCode.Should().NotBeNullOrWhiteSpace();
        sale.Price.Should().Be(120000m);
        sale.Status.Should().Be("Pending");
    }

    [Fact]
    public async Task Purchase_ShouldReturnUnauthorized_WhenUserIsNotAuthenticated()
    {
        // Arrange
        await using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();

        var request = new PurchaseVehicleRequest
        {
            VehicleId = Guid.NewGuid(),
            BuyerCpf = "52998224725"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/sales", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Purchase_ShouldReturnForbidden_WhenUserIsAdmin()
    {
        // Arrange
        await using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();

        client.DefaultRequestHeaders.Add("X-Test-User", "admin-test-001");
        client.DefaultRequestHeaders.Add("X-Test-Role", "admin");

        var request = new PurchaseVehicleRequest
        {
            VehicleId = Guid.NewGuid(),
            BuyerCpf = "52998224725"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/sales", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Purchase_ShouldMoveVehicleFromAvailableToSold()
    {
        // Arrange
        await using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();

        client.DefaultRequestHeaders.Add("X-Test-User", "admin-test-001");
        client.DefaultRequestHeaders.Add("X-Test-Role", "admin");

        var createVehicleResponse = await client.PostAsJsonAsync("/api/vehicles", new CreateVehicleRequest
        {
            Brand = "Honda",
            Model = "Civic",
            Year = 2021,
            Color = "Preto",
            Price = 115000m
        });

        var vehicle = await createVehicleResponse.Content.ReadFromJsonAsync<VehicleResponse>();

        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("X-Test-User", "buyer-test-001");
        client.DefaultRequestHeaders.Add("X-Test-Role", "buyer");

        var purchaseResponse = await client.PostAsJsonAsync("/api/sales", new PurchaseVehicleRequest
        {
            VehicleId = vehicle!.Id,
            BuyerCpf = "52998224725"
        });

        var sale = await purchaseResponse.Content.ReadFromJsonAsync<SaleResponse>();

        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("X-Test-User", "main-service");
        client.DefaultRequestHeaders.Add("X-Test-Role", "admin");

        var paymentResponse = await client.PutAsJsonAsync(
            $"/api/sales/payments/{sale!.PaymentCode}",
            new ProcessPaymentRequest { Status = "Completed" });

        paymentResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        client.DefaultRequestHeaders.Clear();

        // Act
        var availableResponse = await client.GetAsync("/api/vehicles/available");
        var soldResponse = await client.GetAsync("/api/vehicles/sold");

        // Assert
        availableResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        soldResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var availableVehicles = await availableResponse.Content.ReadFromJsonAsync<List<VehicleResponse>>();
        var soldVehicles = await soldResponse.Content.ReadFromJsonAsync<List<VehicleResponse>>();

        availableVehicles.Should().NotBeNull();
        soldVehicles.Should().NotBeNull();

        availableVehicles!.Should().BeEmpty();
        soldVehicles!.Should().HaveCount(1);
        soldVehicles[0].Id.Should().Be(vehicle.Id);
        soldVehicles[0].Status.Should().Be("Sold");
    }

    [Fact]
    public async Task ProcessPayment_ShouldReleaseVehicle_WhenPaymentIsCanceled()
    {
        await using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();

        client.DefaultRequestHeaders.Add("X-Test-User", "admin-test-001");
        client.DefaultRequestHeaders.Add("X-Test-Role", "admin");
        var createResponse = await client.PostAsJsonAsync("/api/vehicles", new CreateVehicleRequest
        {
            Brand = "Ford", Model = "Ka", Year = 2020, Color = "Branco", Price = 50000m
        });
        var vehicle = await createResponse.Content.ReadFromJsonAsync<VehicleResponse>();

        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("X-Test-User", "buyer-test-001");
        client.DefaultRequestHeaders.Add("X-Test-Role", "buyer");
        var purchaseResponse = await client.PostAsJsonAsync("/api/sales", new PurchaseVehicleRequest
        {
            VehicleId = vehicle!.Id,
            BuyerCpf = "52998224725"
        });
        var sale = await purchaseResponse.Content.ReadFromJsonAsync<SaleResponse>();

        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("X-Test-User", "main-service");
        client.DefaultRequestHeaders.Add("X-Test-Role", "admin");
        var cancelResponse = await client.PutAsJsonAsync(
            $"/api/sales/payments/{sale!.PaymentCode}",
            new ProcessPaymentRequest { Status = "Canceled" });

        cancelResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var available = await client.GetFromJsonAsync<List<VehicleResponse>>("/api/vehicles/available");
        available.Should().ContainSingle(item => item.Id == vehicle.Id);
    }
}
