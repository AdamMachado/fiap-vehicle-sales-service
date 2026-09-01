using System.Net;
using System.Net.Http.Json;
using Fiap.VehicleSales.Application.DTOs;
using Fiap.VehicleSales.IntegrationTests.Factory;
using FluentAssertions;

namespace Fiap.VehicleSales.IntegrationTests.Controllers;

public sealed class SalesControllerTests
{
    [Fact]
    public async Task Purchase_ShouldReturnPendingSale_WhenBuyerAndVehicleAreValid()
    {
        await using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();
        var vehicle = await SyncVehicleAsync(client, "Toyota", "Corolla", 120000m);
        Authenticate(client, "buyer-test-001", "buyer");

        var response = await client.PostAsJsonAsync("/api/sales", Purchase(vehicle.Id));
        var sale = await response.Content.ReadFromJsonAsync<SaleResponse>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        sale!.VehicleId.Should().Be(vehicle.Id);
        sale.BuyerId.Should().Be("buyer-test-001");
        sale.BuyerCpf.Should().Be("52998224725");
        sale.PaymentCode.Should().NotBeNullOrWhiteSpace();
        sale.Price.Should().Be(120000m);
        sale.Status.Should().Be("Pending");
    }

    [Fact]
    public async Task Purchase_ShouldReturnUnauthorized_WhenUserIsNotAuthenticated()
    {
        await using var factory = new CustomWebApplicationFactory();
        var response = await factory.CreateClient().PostAsJsonAsync("/api/sales", Purchase(Guid.NewGuid()));
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Purchase_ShouldReturnForbidden_WhenUserIsAdmin()
    {
        await using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();
        Authenticate(client, "admin-test-001", "admin");

        var response = await client.PostAsJsonAsync("/api/sales", Purchase(Guid.NewGuid()));
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task ProcessPayment_ShouldMoveVehicleFromAvailableToSold()
    {
        await using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();
        var vehicle = await SyncVehicleAsync(client, "Honda", "Civic", 115000m);
        Authenticate(client, "buyer-test-001", "buyer");
        var purchaseResponse = await client.PostAsJsonAsync("/api/sales", Purchase(vehicle.Id));
        var sale = await purchaseResponse.Content.ReadFromJsonAsync<SaleResponse>();

        Authenticate(client, "main-software", "vehicle-sales-service");
        var paymentResponse = await client.PutAsJsonAsync(
            $"/api/sales/payments/{sale!.PaymentCode}",
            new ProcessPaymentRequest { Status = "Completed" });
        client.DefaultRequestHeaders.Clear();

        var available = await client.GetFromJsonAsync<List<VehicleResponse>>("/api/vehicles/available");
        var sold = await client.GetFromJsonAsync<List<VehicleResponse>>("/api/vehicles/sold");

        paymentResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        available.Should().BeEmpty();
        sold.Should().ContainSingle(item => item.Id == vehicle.Id && item.Status == "Sold");
    }

    [Fact]
    public async Task ProcessPayment_ShouldReleaseVehicle_WhenPaymentIsCanceled()
    {
        await using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();
        var vehicle = await SyncVehicleAsync(client, "Ford", "Ka", 50000m);
        Authenticate(client, "buyer-test-001", "buyer");
        var purchaseResponse = await client.PostAsJsonAsync("/api/sales", Purchase(vehicle.Id));
        var sale = await purchaseResponse.Content.ReadFromJsonAsync<SaleResponse>();

        Authenticate(client, "main-software", "vehicle-sales-service");
        var cancelResponse = await client.PutAsJsonAsync(
            $"/api/sales/payments/{sale!.PaymentCode}",
            new ProcessPaymentRequest { Status = "Canceled" });
        client.DefaultRequestHeaders.Clear();

        cancelResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var available = await client.GetFromJsonAsync<List<VehicleResponse>>("/api/vehicles/available");
        available.Should().ContainSingle(item => item.Id == vehicle.Id);
    }

    [Fact]
    public async Task ProcessPayment_ShouldReturnForbidden_WhenCallerIsAdmin()
    {
        await using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();
        Authenticate(client, "admin-test-001", "admin");

        var response = await client.PutAsJsonAsync(
            "/api/sales/payments/unknown",
            new ProcessPaymentRequest { Status = "Completed" });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private static PurchaseVehicleRequest Purchase(Guid vehicleId) => new()
    {
        VehicleId = vehicleId,
        BuyerCpf = "52998224725"
    };

    private static async Task<VehicleResponse> SyncVehicleAsync(
        HttpClient client,
        string brand,
        string model,
        decimal price)
    {
        Authenticate(client, "main-software", "vehicle-sales-service");
        var response = await client.PutAsJsonAsync($"/api/internal/vehicles/{Guid.NewGuid()}", new SyncVehicleRequest
        {
            Brand = brand,
            Model = model,
            Year = 2022,
            Color = "Prata",
            Price = price
        });
        return (await response.Content.ReadFromJsonAsync<VehicleResponse>())!;
    }

    private static void Authenticate(HttpClient client, string user, string role)
    {
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("X-Test-User", user);
        client.DefaultRequestHeaders.Add("X-Test-Role", role);
    }
}
