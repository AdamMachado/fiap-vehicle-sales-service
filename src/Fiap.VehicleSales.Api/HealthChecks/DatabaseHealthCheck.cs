using Fiap.VehicleSales.Infrastructure.Persistence;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Fiap.VehicleSales.Api.HealthChecks;

public sealed class DatabaseHealthCheck : IHealthCheck
{
    private readonly VehicleSalesDbContext _dbContext;

    public DatabaseHealthCheck(VehicleSalesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _dbContext.Database.CanConnectAsync(cancellationToken)
                ? HealthCheckResult.Healthy("Banco transacional disponível.")
                : HealthCheckResult.Unhealthy("Banco transacional indisponível.");
        }
        catch (Exception exception)
        {
            return HealthCheckResult.Unhealthy(
                "Falha ao verificar o banco transacional.",
                exception);
        }
    }
}
