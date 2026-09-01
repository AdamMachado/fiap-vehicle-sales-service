using System.Net;
using System.Net.Http.Json;
using Fiap.VehicleSales.Application.DTOs;
using Fiap.VehicleSales.IntegrationTests.Factory;
using FluentAssertions;

namespace Fiap.VehicleSales.IntegrationTests.Controllers;

public sealed class VehiclesControllerTests
{
    [Fact]
    public async Task Sync_ShouldCreateAndUpdateVehicle_WhenCallerIsInternalService()
    {
        await using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();
        AuthenticateAsService(client);
        var vehicleId = Guid.NewGuid();

        var createResponse = await client.PutAsJsonAsync(
            $"/api/internal/vehicles/{vehicleId}",
            Request("Toyota", "Corolla", 120000m));
        var updateResponse = await client.PutAsJsonAsync(
            $"/api/internal/vehicles/{vehicleId}",
            Request("Toyota", "Corolla XEi", 118000m));

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var vehicle = await updateResponse.Content.ReadFromJsonAsync<VehicleResponse>();
        vehicle!.Id.Should().Be(vehicleId);
        vehicle.Model.Should().Be("Corolla XEi");
        vehicle.Price.Should().Be(118000m);
    }

    [Theory]
    [InlineData("buyer")]
    [InlineData("admin")]
    public async Task Sync_ShouldReturnForbidden_WhenCallerIsNotInternalService(string role)
    {
        await using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Test-User", "unauthorized-user");
        client.DefaultRequestHeaders.Add("X-Test-Role", role);

        var response = await client.PutAsJsonAsync(
            $"/api/internal/vehicles/{Guid.NewGuid()}",
            Request("Toyota", "Corolla", 120000m));

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task PublicCreateAndUpdate_ShouldNotExist()
    {
        await using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();

        var create = await client.PostAsJsonAsync("/api/vehicles", Request("Toyota", "Corolla", 120000m));
        var update = await client.PutAsJsonAsync($"/api/vehicles/{Guid.NewGuid()}", Request("Toyota", "Corolla", 120000m));

        create.StatusCode.Should().Be(HttpStatusCode.NotFound);
        update.StatusCode.Should().Be(HttpStatusCode.MethodNotAllowed);
    }

    [Fact]
    public async Task GetAvailable_ShouldReturnVehiclesOrderedByPrice()
    {
        await using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();
        AuthenticateAsService(client);

        await SyncAsync(client, Request("Honda", "Civic", 115000m));
        await SyncAsync(client, Request("Fiat", "Argo", 65000m));
        await SyncAsync(client, Request("Toyota", "Corolla", 120000m));
        client.DefaultRequestHeaders.Clear();

        var response = await client.GetAsync("/api/vehicles/available");
        var vehicles = await response.Content.ReadFromJsonAsync<List<VehicleResponse>>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        vehicles!.Select(vehicle => vehicle.Price).Should().ContainInOrder(65000m, 115000m, 120000m);
    }

    private static Task<HttpResponseMessage> SyncAsync(HttpClient client, SyncVehicleRequest request) =>
        client.PutAsJsonAsync($"/api/internal/vehicles/{Guid.NewGuid()}", request);

    private static SyncVehicleRequest Request(string brand, string model, decimal price) => new()
    {
        Brand = brand,
        Model = model,
        Year = 2022,
        Color = "Prata",
        Price = price
    };

    private static void AuthenticateAsService(HttpClient client)
    {
        client.DefaultRequestHeaders.Add("X-Test-User", "main-software");
        client.DefaultRequestHeaders.Add("X-Test-Role", "vehicle-sales-service");
    }
}
