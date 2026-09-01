using System.Net;
using System.Net.Http.Json;
using Fiap.VehicleSales.Application.DTOs;
using Fiap.VehicleSales.IntegrationTests.Factory;
using FluentAssertions;

namespace Fiap.VehicleSales.IntegrationTests.Controllers;

public sealed class VehiclesControllerTests
{
    [Fact]
    public async Task Create_ShouldReturnCreated_WhenUserIsAdmin()
    {
        // Arrange
        await using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();

        client.DefaultRequestHeaders.Add("X-Test-User", "admin-test-001");
        client.DefaultRequestHeaders.Add("X-Test-Role", "admin");

        var request = new CreateVehicleRequest
        {
            Brand = "Toyota",
            Model = "Corolla",
            Year = 2022,
            Color = "Prata",
            Price = 120000m
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/vehicles", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var vehicle = await response.Content.ReadFromJsonAsync<VehicleResponse>();

        vehicle.Should().NotBeNull();
        vehicle!.Id.Should().NotBeEmpty();
        vehicle.Brand.Should().Be("Toyota");
        vehicle.Model.Should().Be("Corolla");
        vehicle.Status.Should().Be("Available");
    }

    [Fact]
    public async Task Create_ShouldReturnForbidden_WhenUserIsBuyer()
    {
        // Arrange
        await using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();

        client.DefaultRequestHeaders.Add("X-Test-User", "buyer-test-001");
        client.DefaultRequestHeaders.Add("X-Test-Role", "buyer");

        var request = new CreateVehicleRequest
        {
            Brand = "Toyota",
            Model = "Corolla",
            Year = 2022,
            Color = "Prata",
            Price = 120000m
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/vehicles", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetAvailable_ShouldReturnVehiclesOrderedByPrice()
    {
        // Arrange
        await using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();

        client.DefaultRequestHeaders.Add("X-Test-User", "admin-test-001");
        client.DefaultRequestHeaders.Add("X-Test-Role", "admin");

        await client.PostAsJsonAsync("/api/vehicles", new CreateVehicleRequest
        {
            Brand = "Honda",
            Model = "Civic",
            Year = 2021,
            Color = "Preto",
            Price = 115000m
        });

        await client.PostAsJsonAsync("/api/vehicles", new CreateVehicleRequest
        {
            Brand = "Fiat",
            Model = "Argo",
            Year = 2020,
            Color = "Branco",
            Price = 65000m
        });

        await client.PostAsJsonAsync("/api/vehicles", new CreateVehicleRequest
        {
            Brand = "Toyota",
            Model = "Corolla",
            Year = 2022,
            Color = "Prata",
            Price = 120000m
        });

        client.DefaultRequestHeaders.Clear();

        // Act
        var response = await client.GetAsync("/api/vehicles/available");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var vehicles = await response.Content.ReadFromJsonAsync<List<VehicleResponse>>();

        vehicles.Should().NotBeNull();
        vehicles.Should().HaveCount(3);

        vehicles![0].Price.Should().Be(65000m);
        vehicles[1].Price.Should().Be(115000m);
        vehicles[2].Price.Should().Be(120000m);
    }
}