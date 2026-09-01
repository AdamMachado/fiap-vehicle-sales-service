using System.Net;
using Fiap.VehicleSales.IntegrationTests.Factory;
using FluentAssertions;

namespace Fiap.VehicleSales.IntegrationTests.HealthChecks;

public sealed class HealthCheckTests
{
    [Theory]
    [InlineData("/health/live")]
    [InlineData("/health/ready")]
    public async Task HealthCheck_ShouldReturnOk(string endpoint)
    {
        await using var factory = new CustomWebApplicationFactory();
        var response = await factory.CreateClient().GetAsync(endpoint);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
